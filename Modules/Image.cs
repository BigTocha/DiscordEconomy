using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using QRCoder;
using ZXing;

namespace DiscordEconomy.Modules
{
    [Group("Image")]
    public class Image : ModuleBase
    {
        QRCodeGenerator _generator = new QRCodeGenerator();
        BarcodeReader _reader = new BarcodeReader();

        [Command("QR")]
        public async Task GetQr([Remainder]string text)
        {
            QRCodeData data = _generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode code = new QRCode(data);
            Bitmap image = code.GetGraphic(1);
            string fileName = Path.ChangeExtension(Path.GetTempFileName(), ".png");
            image.Save(fileName);
            await Context.Channel.SendFileAsync(fileName);
        }

        [Command("ParseQR")]
        public async Task ParseQr()
        {
            IAttachment attachment = Context.Message.Attachments.FirstOrDefault();
            if (attachment == null)
                throw new Exception("Нет вложений");
            WebRequest request = WebRequest.Create(attachment.Url);
            WebResponse response = request.GetResponse();
            Result res = _reader.Decode(new Bitmap(response.GetResponseStream()));
            await Context.Channel.SendMessageAsync(res.Text);
        }

        [Command("WriteText")]
        public async Task WriteText([Remainder]string text)
        {
            IAttachment attachment = Context.Message.Attachments.FirstOrDefault();
            if (attachment == null)
                throw new Exception("Нет вложений");
            WebRequest request = WebRequest.Create(attachment.Url);
            WebResponse response = request.GetResponse();
            Bitmap image = new Bitmap(response.GetResponseStream());
            QRCodeData data = _generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode code = new QRCode(data);
            Bitmap codeImage = code.GetGraphic(1);
            Bitmap resImage = Common.Common.IncludeInfoToBitmap(image, codeImage);
            string fileName = Path.ChangeExtension(Path.GetTempFileName(), ".png");
            resImage.Save(fileName);
            await Context.Channel.SendFileAsync(fileName);
        }

        [Command("GetInfo")]
        public async Task GetInfo()
        {
            IAttachment attachment = Context.Message.Attachments.FirstOrDefault();
            if (attachment == null)
                throw new Exception("Нет вложений");
            WebRequest request = WebRequest.Create(attachment.Url);
            WebResponse response = request.GetResponse();
            Bitmap image = new Bitmap(response.GetResponseStream());
            Bitmap resImage = Common.Common.GetInfoFromBitmap(image);
            string fileName = Path.ChangeExtension(Path.GetTempFileName(), ".png");
            resImage.Save(fileName);
            await Context.Channel.SendFileAsync(fileName);
        }

        [Command("GetText")]
        public async Task GetText()
        {
            IAttachment attachment = Context.Message.Attachments.FirstOrDefault();
            if (attachment == null)
                throw new Exception("Нет вложений");
            WebRequest request = WebRequest.Create(attachment.Url);
            WebResponse response = request.GetResponse();
            Bitmap image = new Bitmap(response.GetResponseStream());
            Bitmap resImage = Common.Common.GetInfoFromBitmap(image);
            Result res = _reader.Decode(resImage);
            await Context.Channel.SendMessageAsync(res.Text);
        }
    }
}
