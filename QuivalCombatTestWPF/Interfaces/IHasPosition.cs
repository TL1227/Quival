using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuivalCombatTestWPF.Interfaces
{
    interface IHasPosition
    {
        void SetPos(Position postition);
        Position GetPos();
    }
}
