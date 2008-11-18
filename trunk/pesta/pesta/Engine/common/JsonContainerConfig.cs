using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Jayrock.Json;
using Jayrock.Json.Conversion;

namespace Pesta
{
    class JsonContainerConfig : ContainerConfig
    {
        
          public static readonly char FILE_SEPARATOR = ',';
          public static readonly String PARENT_KEY = "parent";
          // TODO: Rename this to simply "container", gadgets.container is unnecessary.
          public static readonly String CONTAINER_KEY = "gadgets.container";

          private readonly Dictionary<String, JsonObject> config;

      /**
       * Creates a new, empty configuration.
       * @param containers
       * @throws ContainerConfigException
       */
          public static readonly JsonContainerConfig Instance =
                  new JsonContainerConfig(AppDomain.CurrentDomain.BaseDirectory + @"config\container.js");

        protected JsonContainerConfig(String containers)
        {
            config = new Dictionary<String, JsonObject>();
            if (containers != null) 
            {
                loadContainers(containers);
            }
        }

        public override ICollection<String> getContainers()
        {
            return config.Keys;
        }

        public override Object getJson(String container, String parameter) 
        {
            JsonObject data = config[container];
            if (data == null)
            {
                return null;
            }
            if (parameter == null)
            {
                return data;
            }

            try
            {
                foreach (String param in parameter.Split('/'))
                {
                    Object next = data[param];
                    if (next is JsonObject)
                    {
                        data = (JsonObject)next;
                    }
                    else
                    {
                        return next;
                    }
                }
                return data;
            }
            catch (JsonException e)
            {
                return null;
            }
        }

        public override String get(String container, String parameter) 
        {
            Object data = getJson(container, parameter);
            return data == null ? null : data.ToString();
        }

        public override JsonObject getJsonObject(String container, String parameter)
        {
            Object data = getJson(container, parameter);
            if (data is JsonObject)
            {
                return (JsonObject)data;
            }
            return null;
        }

        public override JsonArray getJsonArray(String container, String parameter) 
        {
            Object data = getJson(container, parameter);
            if (data is JsonArray)
            {
                return (JsonArray)data;
            }
            return null;
        }

      /**
       * Loads containers from the specified resource. Follows the same rules
       * as {@code JsFeatureLoader.loadFeatures} for locating resources.
       *
       * @param path
       * @throws ContainerConfigException
       */
        private void loadContainers(string path)
        {
            string json;
            using (StreamReader sr = new StreamReader(path))
            {
                json = sr.ReadToEnd();
            }
            JsonObject contents = JsonConvert.Import(json) as JsonObject;
            JsonArray containers = contents[CONTAINER_KEY] as JsonArray;
            for (int i = 0; i < containers.Length; i++)
            {
                // Copy the default object and produce a new one.
                String container = containers.GetString(i);
                config.Add(container, contents);
            }
        }
    }
}
