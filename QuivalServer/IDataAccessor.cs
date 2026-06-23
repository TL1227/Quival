using QuivalLogicEngine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuivalServer
{
    internal interface IDataAccessor
    {
        Card? GetCard(string uniqueId);
    }
}
