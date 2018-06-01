using wServer.logic.behaviors.Drakes;
using wServer.logic.behaviors;
using wServer.logic.transitions;

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private _ Drakes = () => Behav()
            .Init("White Drake",
                new State(
                    new DrakeFollow(),
                    new WhiteDrakeAttack()
                )
            )
            .Init("Blue Drake",
                new State(
                    new DrakeFollow(),
                    new BlueDrakeAttack()
                )
            )
            .Init("Purple Drake",
                new State(
                    new DrakeFollow(),
                    new PurpleDrakeAttack()
                )
            )
            .Init("Orange Drake",
                new State(
                    new DrakeFollow(),
                    new OrangeDrakeAttack()
                )
            )
            .Init("Yellow Drake",
                new State(
                    new DrakeFollow(),
                    new YellowDrakeAttack()
                )
            )
            .Init("Green Drake",
                new State(
                    new DrakeFollow(),
                    new GreenDrakeAttack()
                )
            )



        .Init("Tower1",
            new State(
                new State(
                    new TowerAttack(200, coolDown: 400),
                    new TimedTransition(7000, "die")
                    ),
                new State("die",
                        new Decay()
                    )
                )
            )
        .Init("Tower2",
            new State(
                new State(
                    new TowerAttack(250, coolDown: 400),
                    new TimedTransition(7000, "die")
                    ),
                new State("die",
                        new Decay()
                    )
                )
            )
        .Init("Tower3",
            new State(
                new State(
                    new TowerAttack(300, coolDown: 400),
                    new TimedTransition(7000, "die")
                    ),
                new State("die",
                        new Decay()
                    )
                )
            )
        .Init("Tower4",
            new State(
                new State(
                    new TowerAttack(400, coolDown: 400),
                    new TimedTransition(7000, "die")
                    ),
                new State("die",
                        new Decay()
                    )
                )
            )
        .Init("Tower5",
            new State(
                new State(
                    new TowerAttack(550, coolDown: 400),
                    new TimedTransition(8000, "die")
                    ),
                new State("die",
                        new Decay()
                    )
                )
            )
        .Init("Tower6",
            new State(
                new State(
                    new TowerAttack(750, coolDown: 400),
                    new TimedTransition(8000, "die")
                    ),
                new State("die",
                        new Decay()
                    )
                )
            )
        .Init("TowerCata",
            new State(
                new State(
                    new TowerAttack(900, coolDown: 400, color: 0x880088, trueDamage: true),
                    new TimedTransition(8000, "die")
                    ),
                new State("die",
                        new Decay()
                    )
                )
            );
    }
}