using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenSlot.JobGenerator
{
    class Grid5kThreeJobGenerator : Grid5kOneJobGenerator, IJobGenerator
    {
        public Grid5kThreeJobGenerator() : base() {

            this.month = "Aug";
            this.day = 3;
        }
    }
}
