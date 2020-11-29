using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordEconomy.Data;

namespace DiscordEconomy.Modules
{
    [Group("User")]
    public class UserC : ModuleBase
    {
        [Command("Profile"), Summary("Описание параметров пользователя")]
        public async Task Profile()
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
            await
                Context.Channel.SendMessageAsync(
                    $"{currentUser.Id}:\r\nБаланс: {currentUser.Money:F1}\r\nПокупок: {Core.It.BuyList.Count(x => x.User == currentUser)}");
        }
    }
}
