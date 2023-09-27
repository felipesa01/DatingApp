// using System.Collections.Generic;
// using System.ComponentModel;
// using System.Linq.Expressions;
// using System.Runtime.CompilerServices;
// using API.Entities;
// using API.Models;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using NetTopologySuite.Operation.Buffer;

// namespace API.Controllers
// {
//     public class TestController : BaseApiController
//     {

//         public System.Text.Json.Serialization.JsonNumberHandling NumberHandling { get; set; }

//         private readonly PmspgeoContext _context;

//         public TestController(PmspgeoContext context)
//         {
//             this._context = context;
//         }


//         [HttpGet("{z}/{x}/{y}")]
//         public async Task<ActionResult<object>> GetMVT(int x, int y, int z, string format)
//         {
//             var tile = new Tile();
//             tile.X = x;
//             tile.Y = y;
//             tile.Zoom = z;

//             if (!TileIsValid(tile)) return BadRequest("Invalid tile path");

//             var EnvDict = TileToEnvelope(tile);
//             var Sql = EnvelopeToSQL(EnvDict);
//             var SqlFormated = FormattableStringFactory.Create(Sql);
//             var Pbf = await _context.Database.SqlQuery<byte[]>(SqlFormated).ToListAsync(); 

//             return File(Pbf.SingleOrDefault(), "application/vnd.mapbox-vector-tile");


//         }

//         public static bool TileIsValid(Tile tile)
//         {

//             var size = Math.Pow(2, tile.Zoom);

//             if (tile.X >= size || tile.Y >= size) return false;

//             if (tile.X < 0 || tile.Y < 0) return false;

//             return true;

//         }


//         public static Dictionary<string, double> TileToEnvelope(Tile tile)
//         {
//             // Width of world in EPSG:3857
//             var worldMercMax = 20037508.3427892;
//             var worldMercMin = -1 * worldMercMax;
//             var worldMercSize = worldMercMax - worldMercMin;
//             // Width in tiles
//             var worldTileSize = Math.Pow(2, tile.Zoom);
//             // Tile width in EPSG:3857
//             var tileMercSize = worldMercSize / worldTileSize;
//             // Calculate geographic bounds from tile coordinates
//             // XYZ tile coordinates are in "image space" so origin is
//             // top-left, not bottom right
//             var env = new Dictionary<string, double>
//         {
//             {"xmin", worldMercMin + tileMercSize * tile.X },
//             {"xmax", worldMercMin + tileMercSize * (tile.X + 1)},
//             {"ymin", worldMercMax - tileMercSize * (tile.Y + 1)},
//             {"ymax", worldMercMax - tileMercSize * tile.Y}

//         };

//             return env;
//         }

//         public static Dictionary<string, double> TileToEnvelopeWGS84(Tile tile)
//         {
//             // Width of world in EPSG:3857
//             var worldMercMax = 20037508.3427892;
//             var worldMercMin = -1 * worldMercMax;
//             var worldMercSize = worldMercMax - worldMercMin;
//             // Width in tiles
//             var worldTileSize = Math.Pow(2, tile.Zoom);
//             // Tile width in EPSG:3857
//             var tileMercSize = worldMercSize / worldTileSize;
//             // Calculate geographic bounds from tile coordinates
//             // XYZ tile coordinates are in "image space" so origin is
//             // top-left, not bottom right
//             var env = new Dictionary<string, double>
//         {
//             {"xmin", worldMercMin + tileMercSize * tile.X },
//             {"xmax", worldMercMin + tileMercSize * (tile.X + 1)},
//             {"ymin", worldMercMax - tileMercSize * (tile.Y + 1)},
//             {"ymax", worldMercMax - tileMercSize * tile.Y}

//         };

//             return env;

//         }

//         public static string EnvelopeToBoundsSQL(Dictionary<string, double> dict)
//         {
//             var densifyFactor = 4;
//             dict.Add("segSize", (dict.GetValueOrDefault("xmax") - dict.GetValueOrDefault("xmin")) / densifyFactor);
//             var SqlTmpl = String.Format("ST_Segmentize(ST_MakeEnvelope({0}, {1}, {2}, {3}, 3857),{4})",
//             dict.GetValueOrDefault("xmin"), dict.GetValueOrDefault("ymin"), dict.GetValueOrDefault("xmax"), dict.GetValueOrDefault("ymax"), dict.GetValueOrDefault("segSize"));

//             return SqlTmpl;
//         }


//         public static string EnvelopeToSQL(Dictionary<string, double> dict)
//         {

//             var Table = "lotes";
//             var Srid = "31983";
//             var GeomCol = "geom";
//             var AttrCol = "ic";
//             var Env = EnvelopeToBoundsSQL(dict);
            
//             var SqlTmpl = @"WITH 
//                             bounds AS (SELECT {0} AS geom, {0}::box2d AS b2d),mvtgeom AS (
//                                 SELECT ST_AsMVTGeom(ST_Transform(t.{1}, 3857), bounds.b2d) AS geom, 
//                                     {2}
//                                 FROM {3} t, bounds
//                                 WHERE ST_Intersects(t.{1}, ST_Transform(bounds.geom, {4}))
//                             ) 
//                             SELECT ST_AsMVT(mvtgeom.*) FROM mvtgeom";

//             return String.Format(SqlTmpl, Env, GeomCol, AttrCol, Table, Srid);
//         }

//     }
// }