using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppVKPostsToTelegramMessageParser
{
    internal class VKParser
    {
        private const string xPathEpressionPost = "//div[@class='post_info']";
        private const string xPathEpressionPostText = "//div[@class='wall_post_text']"; ///ancestor::div[not(contains(@class,'own'))]
        private HtmlWeb _htmlWeb;

        private string _url = "";
        private bool _hasPinnedPost = false;

        public VKParser()
        {
            _htmlWeb = new HtmlWeb();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _htmlWeb.OverrideEncoding = Encoding.GetEncoding("windows-1251");
        }

        public string GetPost(int num)
        {
            if (_url == "")
            {
                return BotLogic.emptyUrlMessage;
            }

            var document = _htmlWeb.Load(_url);

            if (document == null)
            {
                return BotLogic.undefinedUrlMessage;
            }

            if (_hasPinnedPost) { num++; }

            var allPostsTextHtml = document.DocumentNode.SelectNodes(xPathEpressionPostText);

            if (allPostsTextHtml.Count == 0) { return BotLogic.noPostsMessage; }

            if (allPostsTextHtml.Count >= num) { return BotLogic.postNotFoundMessage; }

            string postTextHtml = allPostsTextHtml[num].InnerHtml;

            postTextHtml = postTextHtml.Replace("<br>", "\n").Replace("&quot;", "\"");

            StringReader reader = new StringReader(postTextHtml);
            string postText = "";

            bool isWrite = true;
            while (true)
            {
                int symInt = reader.Read();
                
                if (symInt != -1)
                {
                    char sym = (char)symInt;

                    if (sym == '<')
                    {
                        isWrite = false;
                    }
                    else if (sym == '>')
                    {
                        isWrite = true;
                    }
                    else
                    {
                        if (isWrite)
                        {
                            postText += sym;
                        }
                    }
                }
                else { break; }
            }

            reader.Close();

            postText = postText.Replace("&gt;", ">").Replace("&it;", "<").Replace("&#33;", "!").Replace("Показать полностью...", "").Replace("&#8594;", "→");

            return postText;
        }

        public void SetUrl(string url)
        {
            _url = url;
        }

        public void SetPinnedPostState(bool hasPinnedPost)
        {
            _hasPinnedPost = hasPinnedPost;
        }
    }
}
