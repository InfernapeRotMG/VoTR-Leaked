/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using wServer.realm;
using wServer.logic.behaviors;
using wServer.logic.loot;
using wServer.logic.transitions;

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private _ TreasureCave = () => Behav()
            .Init("Golden Oryx Effigy",
             new State(
                    new State("default",
            new PlayerWithinTransition(8, "setsize1")
                        ),
                    new State("setsize1",
                        new ChangeSize(20, 165),
                        new TimedTransition(1100, "spawnminions1")
                        ),
                    new State("spawnminions1",
                        new ConditionalEffect(ConditionEffectIndex.Invulnerable, true),
                        new TossObject("Gold Planet", range: 9.9, angle: 90),
                        new TossObject("Gold Planet", range: 9.9, angle: 50),
                        new TossObject("Gold Planet", range: 9.9, angle: 360),
                        new TossObject("Gold Planet", range: 9.9, angle: 320),
                        new TossObject("Gold Planet", range: 9.9, angle: 270),
                        new TossObject("Gold Planet", range: 9.9, angle: 220),
                        new TossObject("Gold Planet", range: 9.9, angle: 180),
                        new TossObject("Gold Planet", range: 9.9, angle: 130),
                        new TossObject("Treasure Oryx Defender", range: 2.0, angle: 90),
                        new TossObject("Treasure Oryx Defender", range: 2.0, angle: 360),
                        new TossObject("Treasure Oryx Defender", range: 2.0, angle: 270),
                        new TossObject("Treasure Oryx Defender", range: 2.0, angle: 180),
                        new TimedTransition(750, "setsize2")
                        ),
                      new State("setsize2",
                          new ChangeSize(20, 120),
                          new TimedTransition(250, "checkprotectors")
                           ),
                      new State("checkprotectors",
                          new ConditionalEffect(ConditionEffectIndex.Invulnerable, false),
                          new EntityNotExistsTransition("Treasure Oryx Defender", 50, "blink1")
                                ),
                            new State("blink1",
                                new SetAltTexture(1),
                                new TimedTransition(100, "Grenade")
                                ),
                            new State("Grenade",
                                new Grenade(2, 5, 8, fixedAngle: 225, coolDown: 1000),
                                new Grenade(4, 5, 8, fixedAngle: 315, coolDown: 1000),
                                new Grenade(4, 5, 8, fixedAngle: 45, coolDown: 1000),
                                new Grenade(4, 5, 8, fixedAngle: 135, coolDown: 1000),
                                new Grenade(2, 5, 2, fixedAngle: 270, coolDown: 1300),
                                new Grenade(4, 5, 2, fixedAngle: 0, coolDown: 1300),
                                new Grenade(4, 5, 2, fixedAngle: 90, coolDown: 1300),
                                new Grenade(4, 5, 2, fixedAngle: 180, coolDown: 1300)
                                )))
                       .Init("Treasure Oryx Defender",
                            new State(
                                new Orbit(5, 2, acquireRange: 10, target: Golden Oryx Effigy),
                                new Shoot(10, count: 7, projectileIndex: 0, fixedAngle: fixedAngle_RingAttack2)
                                ))
                        .Init("Gold Planet",
                              new State(
                                   new Orbit(5, 8, acquireRange: 10, target: Golden Oryx Effigy)
                                ));
    }
}
*/