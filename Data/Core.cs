using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DiscordEconomy.Data.Migrations;

namespace DiscordEconomy.Data
{
    public class Core
    {
        private static Core _it = new Core();

        public static Core It => _it;

        public Core()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, Configuration>());
            _context = new DataContext();
        }

        private readonly DataContext _context;

        private List<User> _userList;

        private List<Active> _activeList;

        private List<Note> _noteList;
        private List<Buy> _buyList;

        public List<User> UserList => LazyInitializer.EnsureInitialized(ref _userList, () =>
        {
            List<User> res = _context.Users.ToList();
            return res;
        });

        public List<Active> ActiveList => LazyInitializer.EnsureInitialized(ref _activeList, () =>
        {
            List<Active> res = _context.Actives.ToList();
            return res;
        });

        public List<Note> NoteList => LazyInitializer.EnsureInitialized(ref _noteList, () =>
        {
            List<Note> res = _context.Notes.ToList();
            return res;
        });

        public List<Buy> BuyList => LazyInitializer.EnsureInitialized(ref _buyList, () =>
        {
            List<Buy> res = _context.Buys.ToList();
            return res;
        });

        public void AddUser(User user)
        {
            _context.Users.AddOrUpdate(user);
            _context.SaveChanges();
            _userList = null;
        }

        public void AddActive(Active active)
        {
            _context.Actives.AddOrUpdate(active);
            _context.SaveChanges();
            _activeList = null;
        }

        public void AddNote(Note note)
        {
            _context.Notes.AddOrUpdate(note);
            _context.SaveChanges();
            _noteList = null;
        }

        public void AddBuy(Buy buy)
        {
            _context.Buys.AddOrUpdate(buy);
            _context.SaveChanges();
            _buyList = null;
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
