namespace wServer.realm.entities.player
{
    partial class Player
    {
        public void doesLevel(string foodName)
        {
            using (var db = new db.Database())
            {
                var cooklevel = db.GetCookingLevel(Client.Account);
                var setXp = caculateXp(cooklevel, foodName);

                var xpAmount = db.GetCookingXpLevel(Client.Account);

                db.UpdateXpLevel(Client.Account, setXp);
                SendInfo("Your current level is " + cooklevel);
                if (xpAmount >= 1.00M)
                {
                    db.resetXp(Client.Account);
                    CookingLevel = db.UpdateCookingLevel(Client.Account, 1);
                    UpdateCount++;
                    SendHelp("You have leveled up cooking!");
                }
            }
        }

        private decimal xp = 0;
        public decimal endXp = 0;

        private decimal caculateXp(int playerCookingL, string foodType)
        {
            switch (foodType)
            {
                case "Bread":
                    xp = 0.1M;
                    break;

                case "Slice of Pizza":
                    xp = 0.4M;
                    break;

                case "Bowl of Soup":
                    xp = 0.2M;
                    break;

                case "Chocolate Cake":
                    xp = 0.6M;
                    break;

                case "Grilled Cheese Sandwich":
                    xp = 0.3M;
                    break;

                case "Spicy Karambwan":
                    xp = 0.3M;
                    break;

                case "Karambwan":
                    xp = 0.6M;
                    break;

                case "Cookie":
                    xp = 0.1M;
                    break;

                case "Spicy Soup":
                    xp = 0.2M;
                    break;

                case "Salad":
                    xp = 0.2M;
                    break;

                case "Super Full Pizza":
                    xp = 0.5M;
                    break;

                case "Chocolate Bar":
                    xp = 0.1M;
                    break;

                case "Cooked Fish":
                    xp = 0.2M;
                    break;

                case "Spicy Cooked Fish":
                    xp = 0.1M;
                    break;

                case "Cooked Chicken":
                    xp = 0.3M;
                    break;

                case "Coconut Milk":
                    xp = 0.2M;
                    break;

                case "Rice Bowl":
                    xp = 0.1M;
                    break;

                case "Sweet Rice Bowl":
                    xp = 0.4M;
                    break;

                case "Chicken Burger":
                    xp = 0.5M;
                    break;

                case "Drago Spicy Karambwan":
                    xp = 0.7M;
                    break;

                case "Middle of Burger":
                    xp = 0.3M;
                    break;

                case "Slappy Burger":
                    xp = 0.6M;
                    break;

                case "Ghost Burger":
                    xp = 0.7M;
                    break;

                case "Golden Chocolate Bar":
                    xp = 0.6M;
                    break;

                case "Strange Full Pizza":
                    xp = 0.8M;
                    break;

                case "Ghost Pepper":
                    xp = 0.2M;
                    break;

                default:
                    xp = 0;
                    break;
            }
            endXp = xp * 10 / playerCookingL;
            return endXp;
        }
    }
}