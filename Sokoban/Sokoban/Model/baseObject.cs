﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban.Model
{
    public class baseObject
    {

        public virtual char _value
        {
            get { return _value; }
            set { _value = value; }
        }
        public virtual char GetChar()
        {
            return _value;
        }
    }
}