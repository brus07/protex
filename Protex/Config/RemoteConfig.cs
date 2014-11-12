using System.Collections.Generic;
using System.Xml.XPath;

namespace ProtexCore.Config
{
    public class RemoteConfig : OperationsConfig<RemoteOption>
    {
        public RemoteConfig()
        {
            // enshure user cannot
            KnownOptions = new Dictionary<string, RemoteOption>
            {
                {"ssh_port", RemoteOption.SSH_Port},
                {"ssh_hostaddress", RemoteOption.SSH_HostAddress},
                {"ssh_username", RemoteOption.SSH_UserName},
                {"ssh_password", RemoteOption.SSH_Password},
                {"ssh_identity_file", RemoteOption.SSH_IdentityFile},
                {"ssh_identity_passphrase", RemoteOption.SSH_IdentityPassphrase},
                {"user_scripts_folder", RemoteOption.UserScriptsFolder},
                {"tmp_folder", RemoteOption.TmpFolder},
                {"solutions_folder", RemoteOption.SolutionsFolder},
                {"organizer_script", RemoteOption.OrganizerScript},
                {"compiler_script", RemoteOption.CompilerScript},
                {"runner_script", RemoteOption.RunnerScript},
                {"cleanup_script", RemoteOption.CleanUpScript}
            };
        }

        public override void ParseConfig(string pathToXml, string rootElementName)
        {
            Options.Clear();

            XPathDocument doc = new XPathDocument(pathToXml);
            XPathNavigator nav = ((IXPathNavigable) doc).CreateNavigator();

            XPathNodeIterator iter;

            iter = nav.Select(string.Format("/{0}/{1}", rootElementName, "Option"));
            while (iter.MoveNext())
            {
                string nameAttr = iter.Current.GetAttribute("name", string.Empty);
                string valueAttr = iter.Current.GetAttribute("value", string.Empty);

                if (string.IsNullOrEmpty(nameAttr) || string.IsNullOrEmpty(valueAttr))
                    throw new WrongXMLSyntaxException("Empty program options");

                if (KnownOptions.ContainsKey(nameAttr))
                {
                    RemoteOption opt = KnownOptions[nameAttr];

                    if (Options.ContainsKey(opt))
                        throw new WrongXMLSyntaxException("Two properties with same names");

                    Options.Add(opt, valueAttr);
                }
            }
        }
    }
}