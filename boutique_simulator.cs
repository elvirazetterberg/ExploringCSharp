using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RedCrossBoutiqueMS
{

    /// <summary>
    /// A class representing a 
    /// </summary>
    class CustomerSimulator
    {
        static Dictionary<string, Product> mWarehouseDictionary = new Dictionary<string, Product>() {
            {"coffeecup", new Product("coffeecup", 20, 14, "kitchenware")}, 
            {"plate", new Product("plate", 32, 3, "kitchenware")}, 
            {"spork", new Product("spork", 5, 8, "kitchenware")},
            {"vase", new DiscountProduct("vase", 50, 4, "homeware" , 0.2f)},
            {"lavalamp", new DiscountProduct("lavalamp", 299, 5, "homeware", 0.5f)}
        }; // <product.name, product>
        static Checkout mCashier = new Checkout("Emma");

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, customer! Welcome to the Red Cross Second Hand shop!\n");

            while (true)
            {
                Console.Write("Actions: [nothing/browse/purchase]\nWhat would you like to do? ");
                string action = Console.ReadLine(); /* procedure-level variables are camelCase */

                switch (action.ToLower()) {
                    case "nothing":
                    case "n": 
                        return;
                    case "browse":
                        Browse();
                        break;
                    case "purchase":
                        Purchase();
                        break;
                    default:
                        Console.WriteLine("Not a valid action.");
                        break;    
                }

            }
        }

        static void Purchase() 
        {
            Console.Write("What would you like to buy? ");
            string shoppingCart = Console.ReadLine();

            string pattern = @"([0-9]*) *([^,]*)";

            foreach (Match match in Regex.Matches(shoppingCart, pattern))
            {
                string tempQuant = match.Groups[1].ToString();
                if (tempQuant.Length < 1)
                {
                    tempQuant = "1";
                }

                int quantity = int.Parse(tempQuant);
                string item = match.Groups[2].ToString();

                if (mWarehouseDictionary.ContainsKey(item))
                {
                    var prod = mWarehouseDictionary[item];
                    Receipt receipt = mCashier.Transaction(prod, quantity);

                    Console.WriteLine(receipt.ToString());
                }
                else 
                {
                    Console.WriteLine($"Sorry we're out of {item}.");
                }
            
            }
        }

        static void Browse() 
        {
            foreach (KeyValuePair<string, Product> entry in mWarehouseDictionary) 
            {
                var product = entry.Value;
                Console.WriteLine($"We have {product.Quantity}x {product}(s).");
            }
        }
        
    }

    /// <summary>
    /// A class representing a 
    /// </summary>
    class Checkout
    {
        private double dailySales;
        private string worker;

        public Checkout(string worker) /*camelCase parameters*/
        {
            this.worker = worker;
            this.dailySales = 0;
        }

        public Receipt Transaction(Product product, int amount) 
        {
            if (product.UpdateStock(-amount)) 
            {
                dailySales++;
                return new Receipt(product, amount, true, worker); 
            }
            return new Receipt(product, amount, false, worker);
        }
    }

    /// <summary>
    /// A class representing a 
    /// </summary>
    class Product // inherit product but with discount
    {
        public readonly string name;
        protected readonly double price;
        protected int quantity;
        public readonly string category;

        public int Quantity {
            get => this.quantity;
        }

        public Product(string name, double price, int quantity, string category) 
        {
            this.name = name;
            this.price = price;
            this.quantity = quantity;
            this.category = category;
        }

        /// <summary>
        /// Modifies the quantity in stock of a given product by a given amount. 
        /// A negative amount reduces the quantity while a positive amount increases it. 
        /// If the operation results in a negative quantity, the quantity will not 
        /// be modified and false is returned. Otherwise, true is returned. 
        /// </summary>
        /// <param name="amount">Amount to update the quantity with. Can be negative or positive.</param>
        public bool UpdateStock(int amount)
        {
            if (quantity + amount >= 0) 
            {
                quantity += amount;
                return true;
            }
            return false;
        }

        public override string ToString() 
        {
            return this.name;
        }

        public virtual double CalcPrice() 
        {
            return this.price;
        }

    }

    /// <summary>
    /// A class representing a 
    /// </summary>
    class DiscountProduct : Product 
    {
        public float discount;

        public DiscountProduct(string name, double price, int quantity, string category, float discount) : base(name, price, quantity, category) 
        {
            this.discount = discount;
        }

        public override string ToString() 
        {
            return $"({this.discount.ToString("P")}) {this.name}";
        }
        
        public override double CalcPrice() 
        {
            return this.price * (1 - this.discount);
        }
    }

    /// <summary>
    /// A class representing a transaction receipt that prints details of the transaction. It is used when completing a transaction.
    /// </summary>
    class Receipt
    {
        public readonly Product Product;
        public readonly int Quantity;
        public readonly bool IsInStock;
        public readonly string Worker;
        private readonly DateTime mTimestamp;

        public Receipt(Product product, int quantity, bool isInStock, string worker) 
        {
            this.Product = product;
            this.Quantity = quantity;
            this.IsInStock = isInStock;
            this.Worker = worker;
            this.mTimestamp = DateTime.Now;
        }

        public override string ToString() 
        {
            if (this.isInStock) 
            {
                return $"You have purchased {this.Quantity} {this.Product}(s) for {(this.Product.CalcPrice() * this.Quantity).ToString("0.00")} SEK from {this.Worker}.";
            }
            else 
            {
                return $"Purchase declined. We only have {this.Product.Quantity} items left.";
            }
        }
    }
}