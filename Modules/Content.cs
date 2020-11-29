using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Discord;
using Discord.Commands;
using DiscordEconomy.Data;
using ZXing.Client.Result;

namespace DiscordEconomy.Modules
{
    [Group("Content")]
    public class Content : ModuleBase
    {
        [Command("Add"), Summary("Пришлите ценный контент (1 - название контента, 2 - базовый рейтинг (опц.))")]
        public async Task Add([Summary("Название контента")] string name, [Summary("Рейтинг контента")] int rating = 1)
        {
            IUser autor = Context.Message.Author;
            if (Core.It.UserList.All(x => x.Id != autor.Mention))
            {
                User newUser = new User
                {
                    Id = autor.Mention,
                    Rating = 1,
                    Money = 10000
                };
                Core.It.AddUser(newUser);
            }
            User currentUser = Core.It.UserList.First(x => x.Id == autor.Mention);
            if (Context.Message.Attachments.Count == 0)
                await Context.Channel.SendMessageAsync("Не найдено вложений");
            foreach (IAttachment attachment in Context.Message.Attachments)
            {
                Guid id = Guid.NewGuid();

                Active active = new Active
                {
                    Id = id,
                    Name = name,
                    Url = attachment.Url,
                    Rating = rating,
                    Stamp = DateTime.Now,
                    User = currentUser,
                    FileName = attachment.Filename,
                    Value = Common.Common.GetBytesFromUrl(attachment.Url)
                };
                Core.It.AddActive(active);
            }
            await
                Context.Channel.SendMessageAsync(
                    $"{string.Join("\r\n", Context.Message.Attachments.Select(x => x.Url))}\r\nCommand recieved, {rating}");
        }

        private const int _itemsOnPage = 1;

        [Command("Table"), Summary("Текущая таблица контента с рейтингом (опционально - номер страницы)")]
        public async Task Table([Summary("Номер страницы")] int pageNum = 0)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.Author = new EmbedAuthorBuilder()
            {
                IconUrl = Context.User.GetAvatarUrl(),
                Name = Context.User.Username,
            };
            builder.Color = Color.DarkBlue;
            builder.Title = @"Список товаров";

            List<Active> actives = Core.It.ActiveList
                .OrderByDescending(Common.Common.GetActiveRating).Skip(_itemsOnPage * pageNum).Take(_itemsOnPage).ToList();
            IUserMessage message = await Context.Channel.SendMessageAsync(
                string.Empty, false, builder.WithDescription(
                    $@"```
 Page {pageNum + 1}/{(Core.It.ActiveList.Count - 1) / _itemsOnPage + 1}
{new string('-', 60)}
{
                        string.Join("\r\n",
                            actives.Select(
                                x =>
                                    $"|{(_itemsOnPage * pageNum + actives.IndexOf(x) + 1).ToString().PadRight(3)}|{x.Name.PadRight(49)}|{Common.Common.GetActiveRating(x).ToString("C0").PadRight(4)}|"))
                        }
{new string('-', 60)}
```").Build()
                );
            ReactInterface react = new ReactInterface(message, Context.User.Id, new[] { '\u2B05'.ToString(), '\u27A1'.ToString() });
            react.PageCount = Core.It.ActiveList.Count;
            react.PageChangedEvent += async currentPage =>
            {
                actives = Core.It.ActiveList
                    .OrderByDescending(x => x.Rating).Skip(_itemsOnPage * currentPage).Take(_itemsOnPage).ToList();
                await message.ModifyAsync(prop =>
                {
                    prop.Embed = builder.WithDescription(
                        $@"```
 Page {currentPage + 1}/{(Core.It.ActiveList.Count - 1)/_itemsOnPage + 1}
{
                            new string('-', 60)}
{
                            string.Join("\r\n",
                                actives.Select(
                                    x =>
                                        $"|{(_itemsOnPage*currentPage + actives.IndexOf(x) + 1).ToString().PadRight(3)}➤{x.Name.PadRight(49)}:{Common.Common.GetActiveRating(x).ToString("C0").PadRight(4)}|"))
                            }
{new string('-', 60)}
```").Build();
                });
            };


            /*
            await message.AddReactionAsync(new Emoji('\u2B05'.ToString()));
            await message.AddReactionAsync(new Emoji('\u27A1'.ToString()));
            await message.AddReactionAsync(new Emoji(@"🚮"));
            */
        }

        [Command("Buy"), Summary("Покупка контента")]
        public async Task Buy([Summary("Название контента")]string activeName)
        {
            IUser autor = Context.Message.Author;
            if (Core.It.UserList.All(x => x.Id != autor.Mention))
            {
                User newUser = new User
                {
                    Id = autor.Mention,
                    Rating = 1,
                    Money = 10000
                };
                Core.It.AddUser(newUser);
            }
            User currentUser = Core.It.UserList.First(x => x.Id == autor.Mention);
            Active active = Core.It.ActiveList.FirstOrDefault(x => x.Name == activeName);
            if (active == null)
            {
                await Context.Channel.SendMessageAsync(@"Контент с таким наименованием не найден");
                return;
            }
            if (currentUser.Money < Common.Common.GetActiveRating(active))
            {
                await Context.Channel.SendMessageAsync(@"На вашем счету недостаточно средств");
                return;
            }
            await Context.Channel.SendMessageAsync(@"Контент был выслан личным сообщением");
            double activeRating = Common.Common.GetActiveRating(active);
            Buy buy = new Buy
            {
                Id = Guid.NewGuid(),
                Active = active,
                Price = activeRating,
                Stamp = DateTime.Now,
                User = currentUser
            };
            Core.It.AddBuy(buy);
            currentUser.Money -= activeRating;
            active.User.Money += activeRating;
            Core.It.Save();
            IUserMessage message =
                await Context.User
                    .SendMessageAsync($@"Текущий баланс - {currentUser.Money.ToString(@"C0")}");
            await Context.User
                     .SendMessageAsync($@"{active.Name}
{active.Url}");
            ReactInterface react = new ReactInterface(message, Context.User.Id, new []{ @"🔺", @"🔻" });
            Action<bool> voteAction = async vote =>
            {
                Note newNote = new Note
                {
                    Active = active,
                    Id = Guid.NewGuid(),
                    User = currentUser,
                    Value = vote ? 1 : -1
                };
                Core.It.AddNote(newNote);
                await Context.User.SendMessageAsync(
                    $@"Спасибо за оценку! Контент был оценён {(vote ? "положительно" : "отрицательно")
                        }.
Текущий рейтинг контента {active.Name} - {Common.Common.GetActiveRating(active)}");
                react.Dispose();
            };
            react.VotedEvent += voteAction;
        }
    }
}
