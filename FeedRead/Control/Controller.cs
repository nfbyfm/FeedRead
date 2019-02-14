﻿using CodeHollow.FeedReader;
using FeedLister;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FeedRead
{
    public class Controller
    {
        public Controller(MainForm mainForm)
        {

        }

        #region UI-Fucntions
        public void ImportFeedList()
        {
            OpenFileDialog odi = new OpenFileDialog();
            odi.Title = "Import opml-file";
            odi.RestoreDirectory = true;
            odi.Multiselect = false;
            odi.Filter = "ompl-file|*.opml";

            if(odi.ShowDialog() == DialogResult.OK)
            {
                
                try
                {
                    var opmlData = XDocument.Load(odi.FileName, LoadOptions.SetLineInfo);

                    OpmlDocument opmlDocument = OpmlDocument.Create(opmlData, false);

                    
                    if (opmlDocument != null)
                    {
                        foreach (Outline oline in opmlDocument.Body.Outlines)
                        {
                            if(oline.IsBreakPoint)
                            {
                                Console.WriteLine(oline.Title);
                            }
                            else
                            {
                                foreach(Outline o2line in oline.Outlines)
                                {
                                    Console.WriteLine(oline.Title + " -> " + o2line.Title);
                                }
                            }

                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error while importing opml-file. Error: " + ex.Message);
                }
            }
        }

        public void ExportFeedList()
        {

        }

        public void AddNewFeed()
        {
            AddFeedDialog addFeedDialog = new AddFeedDialog(this);
            if(addFeedDialog.ShowDialog() == DialogResult.OK)
            {
                //show next dialog (add Feed to a Group)
                string newFeedUrl = addFeedDialog.feedUrl;

                Console.WriteLine("Controller.AddNewFeed: got new feed-source from user: " + newFeedUrl);

                //show group-Dialog
                SelectGroupDialog sGD = new SelectGroupDialog(GetGroupNames());

                if(sGD.ShowDialog() == DialogResult.OK)
                {
                    //get group-name
                    string groupName = sGD.groupName;

                    //check if it's anew group
                    if(sGD.addNewGroupName)
                    {
                        //create a new group and add the feed to it
                        Console.WriteLine("Controller.AddNewFeed: add feed '" + newFeedUrl + "' to new group '" + groupName + "'.");
                    }
                    else
                    {
                        //find selected group
                        //check if feed already exists and if not, add the new feed to it
                        Console.WriteLine("Controller.AddNewFeed: add feed '" + newFeedUrl + "' to existing group '" + groupName + "'");
                    }
                   
                }
            }
        }

        public void ShowAboutDialog()
        {

        }

        public void ShowSettings()
        {

        }

        #endregion


        #region Feed-related functions
        //get List of all the group-names
        private List<string> GetGroupNames()
        {
            List<string> result = null;

            return result;
        }

        //checks a url for feeds. returns null if none could be found
        public List<string> CheckUrlForFeeds(string url)
        {
            List<string> result = null;

            Uri uriResult;
            bool validURL = Uri.TryCreate(url, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (!string.IsNullOrEmpty(url) && !string.IsNullOrWhiteSpace(url) && validURL)
            {

                var urls = FeedReader.GetFeedUrlsFromUrl(url);
                
                if (urls.Count() < 1) // no url - probably the url is already the right feed url
                {
                    
                    //try to get the actual Feed-Items from the url
                    try
                    {
                        var feed = FeedReader.Read(url);

                        //if successful: add to list
                        result = new List<string>();
                        result.Add(url);

                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Controller.CheckUrlForFeeds: Couldn't get feed from '" + url + "'. Errormessage: " + ex.Message);
                    }
                }
                else
                {
                    result = new List<string>();

                    //add each found feed to list
                    foreach(HtmlFeedLink feedLink in urls)
                    {
                        result.Add(feedLink.Url);
                    }
                }

            }


            return result;
        }


        #endregion
    }
}
