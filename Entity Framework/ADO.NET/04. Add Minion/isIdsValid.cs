using System;
using System.Collections.Generic;
using System.Text;

namespace _04._Add_Minion
{
    public class IsIdsValid
    {
        private object id;
        public IsIdsValid(object id)
        {
            this.id = id;
        }
        public bool isIdValid(object id)
        {
            if (id != null)
                return true;
            return false;
        }
    }
}
