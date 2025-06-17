using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World.Utils
{
    public static class MonsterTickManager
    {
        public static readonly IObservable<long> GlobalMonsterTick = Observable.Interval(TimeSpan.FromSeconds(1)).Publish().RefCount();
    }
}
