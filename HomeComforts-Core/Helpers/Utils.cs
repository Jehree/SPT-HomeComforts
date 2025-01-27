using Comfort.Common;
using EFT;
using Newtonsoft.Json;
using SPT.Common.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace HomeComforts.Helpers
{
    public class Utils
    {
        private static string _assemblyPath = Assembly.GetExecutingAssembly().Location;
        public static string AssemblyFolderPath = Path.GetDirectoryName(_assemblyPath);

        public static Vector3 PlayerFront
        {
            get
            {
                Player player = Singleton<GameWorld>.Instance.MainPlayer;
                return player.Transform.Original.position + player.Transform.Original.forward + (player.Transform.Original.up / 2);
            }
        }

        public static void ExecuteAfterSeconds(int seconds, Action<object> callback, object arg = null)
        {
            StaticManager.BeginCoroutine(ExecuteAfterSecondsRoutine(seconds, callback, arg));
        }

        public static IEnumerator ExecuteAfterSecondsRoutine(int seconds, Action<object> callback, object arg)
        {
            yield return new WaitForSeconds(seconds);
            callback(arg);
        }

        public static void ExecuteNextFrame(Action callback)
        {
            StaticManager.BeginCoroutine(ExecuteNextFrameRoutine(callback));
        }

        public static IEnumerator ExecuteNextFrameRoutine(Action callback)
        {
            yield return null;
            callback();
        }

        public static Quaternion ScaleQuaternion(Quaternion rotation, float scale)
        {
            rotation.ToAngleAxis(out float angle, out Vector3 axis);
            angle *= scale;
            return Quaternion.AngleAxis(angle, axis);
        }

        public static List<GameObject> GetAllDescendants(GameObject parent)
        {
            List<GameObject> descendants = new List<GameObject>();

            foreach (Transform child in parent.transform)
            {
                descendants.Add(child.gameObject);
                descendants.AddRange(GetAllDescendants(child.gameObject));
            }

            return descendants;
        }

        public static T ServerRoute<T>(string url, object data = default(object))
        {
            string json = JsonConvert.SerializeObject(data);
            string req = RequestHandler.PostJson(url, json);
            return JsonConvert.DeserializeObject<T>(req);
        }
        public static string ServerRoute(string url, object data = default(object))
        {
            string json;
            if (data is string)
            {
                Dictionary<string, string> dataDict = new Dictionary<string, string>();
                dataDict.Add("data", (string)data);
                json = JsonConvert.SerializeObject(dataDict);
            }
            else
            {
                json = JsonConvert.SerializeObject(data);
            }

            return RequestHandler.PutJson(url, json);
        }

        public static Mesh GetInvertedMesh(Mesh mesh)
        {
            // Get the original vertices and triangles
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            // Flip the triangles (reverse the winding order)
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int temp = triangles[i];
                triangles[i] = triangles[i + 1];
                triangles[i + 1] = temp;
            }

            Mesh newMesh = GameObject.Instantiate(mesh);

            // Set the flipped triangles back to the mesh
            newMesh.triangles = triangles;

            // Optionally, recalculate the normals, as they will likely be wrong after flipping the triangles
            newMesh.RecalculateNormals();

            // Optionally, recalculate the bounds for proper rendering
            newMesh.RecalculateBounds();

            return newMesh;
        }
    }
}
