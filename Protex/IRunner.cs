using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protex
{
    public interface IRunner
    {
        IResult Run(IRunnerStartInfo runnerStartInfo);
    }
}
