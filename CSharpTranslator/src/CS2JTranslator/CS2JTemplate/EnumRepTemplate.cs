namespace Twiglet.CS2J.Translator.TypeRep
{
	using System;
	using System.Collections.Generic;
	using System.Xml.Serialization;
	using Twiglet.CS2J.Translator.Utils;

	[XmlType("Enum")]
	public class EnumRepTemplate : TypeRepTemplate, IEquatable<EnumRepTemplate>
	{
		private List<EnumMemberRepTemplate> _members = null;
		[XmlArrayItem("Member")]
		public List<EnumMemberRepTemplate> Members {
			get {
				if (_members == null)
					_members = new List<EnumMemberRepTemplate> ();
				return _members;
			}
		}

		private List<CastRepTemplate> _enumCasts = null;
		private List<CastRepTemplate> EnumCasts {
			get {
				if (_enumCasts == null)
				{
					_enumCasts = new List<CastRepTemplate> ();
					CastRepTemplate kast = new CastRepTemplate();
					kast.From = new TypeRepRef("System.Int32");
					kast.Java = "${TYPEOF_totype:16}.values()[${expr}]";
					_enumCasts.Add(kast);
					kast = new CastRepTemplate();
					kast.To = new TypeRepRef("System.Int32");
					kast.Java = "((Enum)${expr}).ordinal()";
					_enumCasts.Add(kast);
				}
				return _enumCasts;
			}
		}

		[XmlArrayItem("Cast")]
		public override List<CastRepTemplate> Casts {
			get {
				if (_casts == null)
				{
					return EnumCasts;
				}
				else
				{
					return _casts;        
				}
			}
		}

		public EnumRepTemplate()
			: base()
		{
			Inherits = new string[] { "System.Enum" };
		}

		public EnumRepTemplate(EnumRepTemplate copyFrom)
			: base(copyFrom)
		{
			foreach (EnumMemberRepTemplate m in copyFrom.Members)
			{
				Members.Add(new EnumMemberRepTemplate(this, m));
			}
		}

		public EnumRepTemplate (List<EnumMemberRepTemplate> ms) : base()
		{
			_members = ms;
		}

		public override ResolveResult Resolve(String name, bool forWrite, DirectoryHT<TypeRepTemplate> AppEnv, bool implicitCast)
		{
			if (Members != null)
			{
				foreach (EnumMemberRepTemplate m in Members)
				{
					if (m.Name == name)
					{
						ResolveResult res = new ResolveResult();
						res.Result = m;
						res.ResultType = this;
						return res;
					}
				}
			}
			return base.Resolve(name, forWrite, AppEnv,implicitCast);
		}
		public override TypeRepTemplate Instantiate(ICollection<TypeRepTemplate> args)
		{
			EnumRepTemplate copy = new EnumRepTemplate(this);
			if (args != null && args.Count != 0) {
				copy.Apply(mkTypeMap(args));
			}
			return copy;
		}
		#region Equality
		public bool Equals (EnumRepTemplate other)
		{
			if (other == null)
				return false;
			
			if (Members != other.Members) {
				if (Members == null || other.Members == null || Members.Count != other.Members.Count)
					return false;
				for (int i = 0; i < Members.Count; i++) {
					if (Members[i] != other.Members[i])
						return false;
				}
			}
			
			return base.Equals(other);
		}

		public override bool Equals (object obj)
		{
			
			EnumRepTemplate temp = obj as EnumRepTemplate;
			
			if (!Object.ReferenceEquals (temp, null))
				return this.Equals (temp);
			return false;
		}

		public static bool operator == (EnumRepTemplate a1, EnumRepTemplate a2)
		{
			return Object.Equals (a1, a2);
		}

		public static bool operator != (EnumRepTemplate a1, EnumRepTemplate a2)
		{
			return !(a1 == a2);
		}

		public override int GetHashCode ()
		{
			int hashCode = base.GetHashCode ();
			if (Members != null) {
				foreach (EnumMemberRepTemplate e in Members) {
					hashCode ^= e.GetHashCode();
				}
			}
			return hashCode;
		}
		#endregion		
	}
}