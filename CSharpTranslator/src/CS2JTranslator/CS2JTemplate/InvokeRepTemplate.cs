namespace Twiglet.CS2J.Translator.TypeRep
{
	using System.Collections.Generic;

	public class InvokeRepTemplate : MethodRepTemplate
	{
		public InvokeRepTemplate()
		{
		}

		public InvokeRepTemplate(TypeRepTemplate parent, MethodRepTemplate copyFrom) 
			: base(parent, copyFrom)
		{
		}

		public InvokeRepTemplate (string retType, string methodName, string[] tParams, List<ParamRepTemplate> pars) : base(retType, methodName, tParams, pars)
		{
		}

//      public override string mkJava()
//      {
//         return "${this:16}.Invoke" +  mkJavaParams(this.Params);
//      }
	}
}