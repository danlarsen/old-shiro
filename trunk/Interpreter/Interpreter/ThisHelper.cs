using System;
using System.Collections.Generic;
using System.Text;

namespace Shiro.Interpreter
{
    public class ThisHelper
    {
        protected List<Token> oldThis = null;
		protected string oldBase = "";
        protected List<string> oldTuple = null;
        protected int oldScope = 0;

        public void StoreThis(Token sym)
        {
            oldThis = sym.list;
            oldTuple = sym.tuple;
            oldScope = sym.scope;
            oldBase = sym.baseClass;
        }

        public Token UnstoreThis(SymbolTable st)
        {
            Token ret = st.table["this"].Clone();
            
            if (oldThis != null)
                st.CreateListSymbol("this", oldScope, oldThis, oldTuple, oldBase);
            else
                st.RemoveSymbol("this");

            return ret;
        }

        public ThisHelper(SymbolTable st)
        {
            if (st.IsInTable("this"))
                StoreThis(st.table["this"]);
        }
    }
}
