using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        
        private readonly ApplicationDbContext _context;
        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var obj = _context.CelestialObjects.Find(id);
            if (obj == null)
                return NotFound();

            obj.Satellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == id).ToList();
            return Ok(obj);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var objs = _context.CelestialObjects.Where(o => o.Name == name).ToList();
            if (!objs.Any())
                return NotFound();

            foreach (var celestialObject in objs)
                celestialObject.Satellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == celestialObject.Id).ToList();
            return Ok(objs);
        }

        [HttpGet()]
        public IActionResult GetAll()
        {
            var objs = _context.CelestialObjects.ToList();
            foreach (var obj in objs)
            {
                obj.Satellites = _context.CelestialObjects.Where(o => o.OrbitedObjectId == obj.Id).ToList();
            }
            return Ok(objs);
        }

        [HttpPost()]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject updatedObject)
        {
            var objToUpdate = _context.CelestialObjects.Find(id);
            if (objToUpdate == null)
                return NotFound();

            objToUpdate.Name = updatedObject.Name;
            objToUpdate.OrbitalPeriod = updatedObject.OrbitalPeriod;
            objToUpdate.OrbitedObjectId = updatedObject.OrbitedObjectId;
            _context.CelestialObjects.Update(objToUpdate);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string newName)
        {
            var objToUpdate = _context.CelestialObjects.Find(id);
            if (objToUpdate == null)
                return NotFound();

            objToUpdate.Name = newName;
            _context.CelestialObjects.Update(objToUpdate);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objsToUpdate = _context.CelestialObjects.Where(o => o.Id == id || o.OrbitedObjectId == id).ToList();
            if (!objsToUpdate.Any())
                return NotFound();

            _context.CelestialObjects.RemoveRange(objsToUpdate);
            _context.SaveChanges();
            return NoContent();
        }

    }
}
