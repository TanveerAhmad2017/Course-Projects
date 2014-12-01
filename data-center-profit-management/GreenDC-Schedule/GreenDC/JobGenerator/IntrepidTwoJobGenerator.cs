using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenSlot.JobGenerator
{
    class IntrepidTwoJobGenerator: IntrepidOneJobGenerator, IJobGenerator
    {

        public IntrepidTwoJobGenerator() : base() {
            this.filePath = Config.dirIntrepidTwo;
        }
    }
}
