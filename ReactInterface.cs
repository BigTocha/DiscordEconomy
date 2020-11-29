using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordEconomy.Annotations;

namespace DiscordEconomy
{
    public class ReactInterface : INotifyPropertyChanged, IDisposable
    {
        public static DiscordSocketClient Client { get; set; }

        public event Action<int> PageChangedEvent = delegate { };
        public event Action<bool> VotedEvent = delegate { };

        private readonly ulong _messageId;
        private readonly ulong _userId;
        private int _currentPage;
        private readonly IUserMessage _message;
        
        private readonly List<string> _activeReactions; 

        public int PageCount { get; set; }

        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (value == _currentPage) return;
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        public ReactInterface(IUserMessage message, ulong userId, IEnumerable<string> activeReactions)
        {
            _messageId = message.Id;
            _userId = userId;
            _activeReactions = activeReactions.ToList();
            _message = message;
            Initialize();
        }

        private async void Initialize()
        {
            Client.ReactionAdded += ClientOnReactionAdded;
            Client.ReactionRemoved += ClientOnReactionRemoved;
            foreach (string reaction in _activeReactions)
            {
                await _message.AddReactionAsync(new Emoji(reaction));
            }
        }

        private Task ClientOnReactionRemoved(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel socketMessageChannel, SocketReaction arg3)
        {
            return Task.Run(() => OnReactionChanged(arg3.MessageId, socketMessageChannel, arg3));
        }

        private Task ClientOnReactionAdded(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel socketMessageChannel, SocketReaction arg3)
        {
            return Task.Run(() => OnReactionChanged(arg3.MessageId, socketMessageChannel, arg3));
        }

        private void IncrementPage()
        {
            if (_currentPage < PageCount - 1)
                CurrentPage++;
        }

        private void DecrementPage()
        {
            if (_currentPage > 0)
                CurrentPage--;
        }

        private void Voted(bool vote)
        {
            VotedEvent(vote);
        }

        public void OnReactionChanged(ulong messageId, ISocketMessageChannel socketMessageChannel,
            SocketReaction reaction)
        {
            if (_messageId != messageId) return;
            if (reaction.UserId != _userId) return;
            if (!_activeReactions.Contains(reaction.Emote.Name)) return;
            switch (reaction.Emote.Name)
            {
                case "➡":
                    IncrementPage();
                    break;
                case "⬅":
                    DecrementPage();
                    break;
                case @"🔻":
                    Voted(false);
                    break;
                case @"🔺":
                    Voted(true);
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void InternalPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(CurrentPage):
                    PageChangedEvent(CurrentPage);
                    break;
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            InternalPropertyChanged(propertyName);
        }

        public void Dispose()
        {
            Client.ReactionAdded -= ClientOnReactionAdded;
            Client.ReactionRemoved -= ClientOnReactionRemoved;
        }
    }
}
