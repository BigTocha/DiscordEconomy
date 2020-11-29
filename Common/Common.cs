using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Discord;
using DiscordEconomy.Data;
using QRCoder;
using ZXing;
using Color = System.Drawing.Color;

namespace DiscordEconomy.Common
{
    public static class Common
    {
        private const int ByteNum = 24;
        private static QRCodeGenerator _generator = new QRCodeGenerator();
        private static BarcodeReader _reader = new BarcodeReader();

        public static Bitmap IncludeInfoToBitmap(Bitmap sourceImage, Bitmap infoImage)
        {
            int sourceHeight = sourceImage.Height;
            int sourceWidth = sourceImage.Width;
            int infoHeight = infoImage.Height;
            int infoWidth = infoImage.Width;
            if (sourceHeight < infoHeight || sourceWidth < infoWidth)
                throw new Exception("Невозможно включить в изображение меньшего размера");
            int heightKoef = sourceHeight/infoHeight;
            int widthKoef = sourceWidth/infoWidth;
            int koef = heightKoef > widthKoef ? widthKoef : heightKoef;
            uint byteItem = 1;
            for (int i = 0; i < ByteNum; i++)
            {
                byteItem *= 2;
            }
            uint invertItem = uint.MaxValue ^ byteItem;
            for (int x = 0; x < infoWidth * koef; x++)
            {
                for (int y = 0; y < infoHeight * koef; y++)
                {
                    uint sourceColor = (uint)sourceImage.GetPixel(x, y).ToArgb();
                    uint infoColor = (uint)infoImage.GetPixel(x/koef, y/koef).ToArgb();
                    bool isBlack = infoColor != uint.MaxValue;
                    if (isBlack)
                    {
                        sourceColor &= invertItem;
                    }
                    else
                    {
                        sourceColor |= byteItem;
                    }
                    sourceImage.SetPixel(x, y, Color.FromArgb( (int) sourceColor));
                }
            }
            return sourceImage;
        }

        public static byte[] GetBytesFromUrl(string url)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            using (MemoryStream stream = new MemoryStream())
            {
                response.GetResponseStream().CopyTo(stream);
                return stream.ToArray();
            }
        }

        public static double GetActiveRating(Active active)
        {
            double plusRating =
                Core.It.NoteList.Where(x => x.Active == active && x.Value == 1).Sum(x => x.User.Rating);
            double minusRating =
                Core.It.NoteList.Where(x => x.Active == active && x.Value == -1).Sum(x => x.User.Rating);
            if (Math.Abs(plusRating - minusRating) < 1e-5) return active.Rating;
            return plusRating < minusRating
                ? active.Rating * Math.Exp(plusRating - minusRating)
                : active.Rating * (1 + (plusRating - minusRating) / (plusRating + minusRating)) +
                  0.1 * (plusRating - minusRating);
        }

        public static Bitmap GetInfoFromBitmap(Bitmap sourceImage)
        {
            int sourceHeight = sourceImage.Height;
            int sourceWidth = sourceImage.Width;
            uint byteItem = 1;
            for (int i = 0; i < ByteNum; i++)
            {
                byteItem *= 2;
            }
            for (int x = 0; x < sourceWidth; x++)
            {
                for (int y = 0; y < sourceHeight; y++)
                {
                    uint sourceColor = (uint)sourceImage.GetPixel(x, y).ToArgb();
                    sourceColor &= byteItem;
                    if (sourceColor == 0)
                    {
                        sourceImage.SetPixel(x, y, Color.Black);
                    }
                    else
                    {
                        sourceImage.SetPixel(x, y, Color.White);
                    }
                }
            }
            return sourceImage;
        }

        public static void CheckContent(IUserMessage message)
        {
            IUser autor = message.Author;
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

        }
    }
}
