using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenSlot.JobGenerator
{
    class Grid5kTwoJobGenerator : Grid5kOneJobGenerator, IJobGenerator
    {

        public Grid5kTwoJobGenerator() : base() {

            this.month = "Jun";
            this.day = 10;
        }
    }
}
