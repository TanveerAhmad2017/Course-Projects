using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenSlot.JobGenerator
{
    class IntrepidThreeJobGenerator: IntrepidOneJobGenerator, IJobGenerator
    {

        public IntrepidThreeJobGenerator(): base(){

            this.filePath = Config.dirIntrepidThree;
        }
    }
}
