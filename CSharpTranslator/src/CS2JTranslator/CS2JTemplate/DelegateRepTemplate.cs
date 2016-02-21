namespace Twiglet.CS2J.Translator.TypeRep
{
	using System;
	using System.Collections.Generic;
	using System.Xml.Serialization;
	using Twiglet.CS2J.Translator.Utils;

	[XmlType("Delegate")]
	public class DelegateRepTemplate : InterfaceRepTemplate, IEquatable<DelegateRepTemplate>
	{
		private InvokeRepTemplate _invoke = null;
		public InvokeRepTemplate Invoke {
			get {
				return _invoke;
			}
			set {
				_invoke = value;
			}
		}

		public DelegateRepTemplate()
			: base()
		{
		}

		public DelegateRepTemplate(DelegateRepTemplate copyFrom)
			: base(copyFrom)
		{
			if (copyFrom.Invoke != null)
			{
				Invoke = new InvokeRepTemplate(this, copyFrom.Invoke);
			}
		}

		public override ResolveResult Resolve(String name, IList<TypeRepTemplate> args, DirectoryHT<TypeRepTemplate> AppEnv, bool implicitCast)
		{

			if ("Invoke" == name && matchParamsToArgs(Invoke.Params, Invoke.ParamArray, args, AppEnv, implicitCast))
			{     
				ResolveResult res = new ResolveResult();
				res.Result = Invoke;
				res.ResultType = BuildType(Invoke.Return, AppEnv);
				return res;
			}
			return base.Resolve(name, args, AppEnv,implicitCast);
		}

		public override void Apply(Dictionary<string,TypeRepTemplate> args)
		{
			Invoke.Apply(args);
			base.Apply(args);
		}
		public override TypeRepTemplate Instantiate(ICollection<TypeRepTemplate> args)
		{
			DelegateRepTemplate copy = new DelegateRepTemplate(this);
			if (args != null && args.Count != 0) {
				copy.Apply(mkTypeMap(args));
			}
			return copy;
		}

		#region Equality
		public bool Equals (DelegateRepTemplate other)
		{
			if (other == null)
				return false;
			
			return Invoke == other.Invoke && base.Equals(other);
		}

		public override bool Equals (object obj)
		{
			
			DelegateRepTemplate temp = obj as DelegateRepTemplate;
			
			if (!Object.ReferenceEquals (temp, null))
				return this.Equals (temp);
			return false;
		}

		public static bool operator == (DelegateRepTemplate a1, DelegateRepTemplate a2)
		{
			return Object.Equals (a1, a2);
		}

		public static bool operator != (DelegateRepTemplate a1, DelegateRepTemplate a2)
		{
			return !(a1 == a2);
		}

		public override int GetHashCode ()
		{
			int hashCode = base.GetHashCode ();

			return (Invoke ?? new InvokeRepTemplate()).GetHashCode() ^ hashCode;
		}
		#endregion	
	}
}