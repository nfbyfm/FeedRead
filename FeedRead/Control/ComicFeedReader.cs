﻿using CodeHollow.FeedReader;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace FeedRead.Control
{
    /// <summary>
    /// class for creating a pseudo-feed from a webpage without a rss-feed
    /// </summary>
    public class ComicFeedReader
    {

        public ComicFeedReader() { }

        public void Read(string pageUrl, ref Feed feed)
        {
            feed = null;

            bool loadSuccess = false;

            string content = "";

            try
            {
                //try to download the main page
                content = new System.Net.WebClient().DownloadString(pageUrl);
                loadSuccess = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while getting comic-webpage: " + Environment.NewLine + ex.Message);
            }

            if(loadSuccess)
            {
                var htmlDoc = new HtmlAgilityPack.HtmlDocument()
                {
                    OptionAutoCloseOnEnd = true,
                    OptionFixNestedTags = true
                };

                htmlDoc.LoadHtml(content);

                if (htmlDoc.DocumentNode != null)
                {
                    feed = new Feed();
                    feed.FeedURL = pageUrl;

                    //get title (if not properly loaded already)
                    HtmlNode titleNode = htmlDoc.DocumentNode.Descendants("title").FirstOrDefault();
                    if (titleNode != null)
                    {
                        //feed.Title = titleNode.Attributes["title"]?.Value?.HtmlDecode();

                        feed.Title = System.Web.HttpUtility.HtmlDecode(titleNode.InnerText);
                    }
                    else
                    {
                        Console.WriteLine("titlenode is null!");
                    }

                    //get chapters
                    string classID_Title = "_2dU-m _1qbNn";
                    string classID_UpdateTime = "_1D0de col-4 col-md-3";

                    HtmlNodeCollection titleNodes = htmlDoc.DocumentNode.SelectNodes("//*[@class='" + classID_Title + "']");
                    HtmlNodeCollection timeNodes = htmlDoc.DocumentNode.SelectNodes("//*[@class='" + classID_UpdateTime + "']");
                    try
                    {
                        if (titleNodes != null && timeNodes != null)
                        {
                            if (timeNodes.Count > 0 && titleNodes.Count > 0)
                            {
                                feed.Items = new List<FeedItem>();

                                //create List of chapters from detected entries with their upload-Dates
                                for (int i = 0; i < Math.Min(timeNodes.Count, titleNodes.Count); i++)
                                {

                                    string suburl = titleNodes[i].OuterHtml;

                                    if (suburl.Contains("/"))
                                    {
                                        suburl = suburl.Remove(0, titleNodes[i].OuterHtml.IndexOf("/"));
                                    }


                                    string url = "https://mangarock.com";

                                    if (suburl.Contains("\""))
                                    {
                                        url += suburl.Remove(suburl.IndexOf("\""));
                                    }

                                    //Console.WriteLine("suburl = " + suburl + "   url = " + url);

                                    
                                    string title = System.Web.HttpUtility.HtmlDecode(titleNodes[i].InnerHtml);//titleNodes[i].Attributes[classID_Title]?.Value?.HtmlDecode();
                                    DateTime updateTime = getDateTime(timeNodes[i].InnerHtml);

                                    FeedItem item = new FeedItem();
                                    item.Id = url;
                                    item.Link = url;
                                    item.PublishingDate = updateTime;
                                    item.Read = false;
                                    item.Title = title;

                                    feed.Items.Add(item);
                                }

                                feed.Updated = true;
                            }
                        }
                        else
                        {
                            Console.WriteLine(feed.Title + ": no Chapter-Nodes detected.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Innerexception 1: " + ex.Message);
                    }
                    /*
                    var links = htmlDoc.DocumentNode?.SelectNodes("//link");
                    if (links == null)
                        yield break; // no links

                    var nodes = links.Where(
                        x => x.Attributes["type"] != null &&
                        (x.Attributes["type"].Value.Contains("application/rss") || x.Attributes["type"].Value.Contains("application/atom")));

                    foreach (var node in nodes)
                    {
                        yield return new HtmlFeedLink()
                        {
                            Title = node.Attributes["title"]?.Value?.HtmlDecode(),
                            Url = node.Attributes["href"]?.Value.HtmlDecode(),
                            FeedType = GetFeedTypeFromLinkType(node.Attributes["type"].Value.HtmlDecode())
                        };
                    }
                    */
                }




                
                

                
            }
             
        }



        private DateTime getDateTime(string innerHtml)
        {
            DateTime dateTime = DateTime.Today;

            try
            {
                if (innerHtml.ToLower().Contains("ago"))
                {
                    string[] splits = innerHtml.Split();
                    int days = Convert.ToInt32(splits[0]);
                    dateTime = DateTime.Today.AddDays(-1 * days);

                    //Console.WriteLine("'ago'-Time: " + time + "  parsed = " + dateTime.ToShortDateString());
                }
                else
                {
                    dateTime = DateTime.Parse(innerHtml);
                    //Console.WriteLine("'normal'-Time: " + time + "  parsed = " + dateTime.ToShortDateString());
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return dateTime;
        }
    }

    

}
