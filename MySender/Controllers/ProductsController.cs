using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using MySender.Models;
using Business;

namespace MySender.Controllers
{
    public class ProductsController : ApiController
    {       
        // POST: api/Products
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await this.NotifyAsync("AddedProduct", product);
            return Ok();

            //return CreatedAtRoute("DefaultApi", new { id = product.Id }, product);
        }

    }
}