using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStoreMVC5.Models;

namespace MyStoreMVC5.Services
{
    public static class CartService
    {
        private const string SessionKey = "MYSTORE_CART";

        public static List<CartLine> Get(HttpSessionStateBase session)
        {
            var cart = session[SessionKey] as List<CartLine>;
            if (cart == null) { cart = new List<CartLine>(); session[SessionKey] = cart; }
            return cart;
        }

        // Always refresh price/name/image from the database; no client-supplied prices.
        public static List<CartLine> Refresh(HttpSessionStateBase session, StoreDbContext db)
        {
            var cart = Get(session);
            var ids = cart.Select(x => x.ProductID).ToList();
            var products = db.Products.Where(x => ids.Contains(x.ProductID)).ToList()
                .ToDictionary(x => x.ProductID);
            cart.RemoveAll(x => !products.ContainsKey(x.ProductID));
            foreach (var line in cart)
            {
                var p = products[line.ProductID];
                line.ProductName = p.ProductName;
                line.ProductImage = p.ProductImage;
                line.UnitPrice = p.ProductPrice;
            }
            return cart;
        }

        public static bool Add(HttpSessionStateBase session, StoreDbContext db, int productId, int quantity)
        {
            if (quantity < 1 || quantity > 99) return false;
            var product = db.Products.Find(productId);
            if (product == null) return false;
            var cart = Get(session);
            var line = cart.FirstOrDefault(x => x.ProductID == productId);
            if (line != null)
            {
                if (line.Quantity + quantity > 99) return false;
                line.Quantity += quantity;
            }
            else
                cart.Add(new CartLine { ProductID = productId, ProductName = product.ProductName,
                    ProductImage = product.ProductImage, UnitPrice = product.ProductPrice, Quantity = quantity });
            return true;
        }

        public static bool Update(HttpSessionStateBase session, int productId, int quantity)
        {
            var line = Get(session).FirstOrDefault(x => x.ProductID == productId);
            if (line == null || quantity < 0 || quantity > 99) return false;
            if (quantity == 0) Get(session).Remove(line);
            else line.Quantity = quantity;
            return true;
        }

        public static void Clear(HttpSessionStateBase session) => session.Remove(SessionKey);
    }
}
