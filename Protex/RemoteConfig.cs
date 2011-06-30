using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;

namespace ProtexCore
{
	public enum RemoteOption 
	{ 
		/* Connection options */
		SSH_HostAddress, SSH_UserName, SSH_Password,
		/* Special folders pathes:
		 * 
		 * UserScriptsFolder - folder with all needed scripts
		 *         TmpFolder - user sources are first saved here
		 *   SolutionsFolder - path to folder with compiled solutions
		*/
		UserScriptsFolder, TmpFolder, SolutionsFolder,
		/* Scripts names:
		 * 
		 * OrganizerScript - moves file from temporary location to 
		 *                   special folder 
		 *  CompilerScript - compiles user sources if they need to
		 *                   be compiled
		 *    RunnerScript - runs user solution in right way with 
		 *                   special security restrictions or without
		 *                   them. Also measures time.
		*/
		OrganizerScript, CompilerScript, RunnerScript
	}
	
	public class WrongXMLSyntaxException : ApplicationException
	{
		public WrongXMLSyntaxException ()
			: base()
		{
		}
		
		public WrongXMLSyntaxException (string message)
			: base(message)
		{
		}
		
		public WrongXMLSyntaxException (string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
	
	public abstract class OperationsConfig<T>
	{
		protected Dictionary<T, string> options;
		protected Dictionary<string, T> known_options;
		
		public OperationsConfig ()
		{
			this.options = new Dictionary<T, string> ();
			// just to be clear
			this.known_options = null;
		}
		
		/// <summary>
		/// Gets the <see cref="ProtexCore.OperationsConfig"/> with the specified option.
		/// </summary>
		/// <param name='option'>
		/// Option.
		/// </param>
		public string this [T option]
		{
			get { return this.options[option]; }
		}
		
		public abstract void ParseConfig (string pathToConfig, string rootElementName);
	}
	
	public class RemoteConfig : OperationsConfig<RemoteOption>
	{
		public RemoteConfig () 
			: base()
		{
            // enshure user cannot
			this.known_options = new Dictionary<string, RemoteOption> ()
			{
				{"ssh_hostaddress", RemoteOption.SSH_HostAddress},
				{"ssh_username", RemoteOption.SSH_UserName},
				{"ssh_password", RemoteOption.SSH_Password},
				{"user_scripts_folder", RemoteOption.UserScriptsFolder},
				{"tmp_folder", RemoteOption.TmpFolder},
				{"solutions_folder", RemoteOption.SolutionsFolder},
				{"organizer_script", RemoteOption.OrganizerScript},
				{"compiler_script", RemoteOption.CompilerScript},
				{"runner_script", RemoteOption.RunnerScript}
			};
		}
		
		public override void ParseConfig (string pathToXML, string rootElementName)
		{
			options.Clear ();
			
			XPathDocument doc = new XPathDocument (pathToXML);
			XPathNavigator nav = ((IXPathNavigable)doc).CreateNavigator ();
			
			XPathNodeIterator iter = nav.Select (string.Format ("/{0}", rootElementName));
			
			iter = nav.Select (string.Format ("/{0}/{1}", rootElementName, "Option"));
			while (iter.MoveNext ())
			{
				string nameAttr = iter.Current.GetAttribute ("name", string.Empty);
				string valueAttr = iter.Current.GetAttribute ("value", string.Empty);
				
				if (string.IsNullOrEmpty (nameAttr) || string.IsNullOrEmpty (valueAttr))
					throw new WrongXMLSyntaxException ("Empty program options");
				
				if (this.known_options.ContainsKey (nameAttr))
				{
					RemoteOption opt = this.known_options[nameAttr];
					
					if (this.options.ContainsKey (opt))
						throw new WrongXMLSyntaxException ("Two properties with same names");
					
					this.options.Add (opt, valueAttr);
				}
			}
		}
	}
	
	public class LocalConfig : OperationsConfig<object>
	{
		public LocalConfig ()
			:base()
		{
		}
		
		public override void ParseConfig (string pathToXML, string rootElementName)
		{
			throw new NotImplementedException ();
		}
	}
}
