using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Palantir
{
    // 3-depth structure
    // root(configuration) - section - group - item(key:value)

    public class ConfigManager
    {
        private string Url;
        private XDocument xdoc;

        public ConfigManager(string url)
        {
            Url = url;

            Init();
        }

        public void Init()
        {
            xdoc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            XElement xroot = new XElement("configuration");
            xdoc.Add(xroot);
        }

        // add/set
        public void AddSection(string section)
        {
            XElement xroot = xdoc.Element("configuration");
            XElement xsection = new XElement(section);
            XElement xcontain = xroot.Element(section);
            
            if(xcontain == null)
                xroot.Add(xsection);
        }

        public void AddGroup(string sectionName, string groupName)
        {
            XElement xroot = xdoc.Element("configuration");
            XElement xsection = xroot.Element(sectionName);

            if (xsection != null)
            {
                XElement xgroup = xsection.Element(groupName);
                if (xgroup == null)
                {
                    xgroup = new XElement(groupName);
                    xsection.Add(xgroup);
                }
            }
        }

        public void AddUpdate(string sectionName, string groupName, string key, string value)
        {
            AddGroup(sectionName, groupName);

            XElement xroot = xdoc.Element("configuration");
            XElement xsection = xroot.Element(sectionName);

            if (xsection != null)
            {
                XElement xgroup = xsection.Element(groupName);
                if(xgroup == null)
                {
                    xgroup = xsection.Element(groupName);
                    xgroup.Add(new XElement(key, value));
                }
                else
                {
                    xgroup.Add(new XElement(key, value));
                }
            }
        }

        // get
        XElement GetSection(string sectionName)
        {
            XElement xroot = xdoc.Element("configuration");
            return xroot.Element(sectionName);
        }

        XElement GetGroup(string sectionName, string groupName)
        {
            XElement xroot = xdoc.Element("configuration");
            XElement xsection = xroot.Element(sectionName);

            if(xsection != null)
            {
                return xsection.Element(groupName);
            }
            return null;
        }

        public List<string> GetGroups(string sectionName)
        {
            List<string> groups = new List<string>();
            XElement xsection = GetSection(sectionName);

            if(xsection != null)
            {
                List<XElement> groupList = xsection.Elements().ToList();
                foreach(XElement elem in groupList)
                {
                    groups.Add(elem.Name.ToString());
                }
            }

            return groups;
        }

        public List<string> GetItems(string sectionName, string groupName, string key)
        {
            List<string> items = new List<string>();
            XElement xgroup = GetGroup(sectionName, groupName);

            if(xgroup != null)
            {
                List<XElement> itemList = xgroup.Elements(key).ToList();
                foreach(XElement elem in itemList)
                {
                    items.Add(elem.Value);
                }
            }

            return items;
        }

        public bool ChangeGroupName(string sectionName, string from, string to)
        {
            XElement xgroup1 = GetGroup(sectionName, from);
            XElement xgroup2 = GetGroup(sectionName, to);

            if(xgroup2 != null) // already has same name in the node
            {
                return false;
            }

            if (xgroup1 != null)
            {
                xgroup1.Name = to;
                return true;
            }
            return false;
        }

        public void RemoveGroup(string sectionName, string groupName)
        {
            XElement xgroup = GetGroup(sectionName, groupName);
            xgroup.Remove();
        }

        public void RemoveItem(string sectionName, string groupName, string key, string value)
        {
            List<string> items = new List<string>();
            XElement xgroup = GetGroup(sectionName, groupName);

            if (xgroup != null)
            {
                List<XElement> itemList = xgroup.Elements(key).ToList();
                foreach (XElement elem in itemList)
                {
                    if(elem.Name == key && elem.Value == value)
                    {
                        elem.Remove();
                    }
                }
            }
        }

        public void Save()
        {
            xdoc.Save(Url);
        }

        public void Load()
        {
            xdoc = XDocument.Load(Url);
        }

        public void Load(string url)
        {
            xdoc = XDocument.Load(url);
        }
    }

}
