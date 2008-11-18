using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jayrock.Json;
using URI = System.Uri;

namespace Pesta
{
    public class Processor
    {
        private readonly GadgetSpecFactory gadgetSpecFactory;
        private readonly VariableSubstituter substituter;
        private readonly ContainerConfig containerConfig;
        private readonly GadgetBlacklist blacklist;
        public static readonly Processor Instance = new Processor();
        protected Processor()
        {
            this.gadgetSpecFactory = DefaultGadgetSpecFactory.Instance;
            this.substituter = new VariableSubstituter();
            this.blacklist = new BasicGadgetBlacklist("");
            this.containerConfig = JsonContainerConfig.Instance;
        }

        /**
        * Process a single gadget. Creates a gadget from a retrieved GadgetSpec and context object,
        * automatically performing variable substitution on the spec for use elsewhere.
        *
        * @throws ProcessingException If there is a problem processing the gadget.
        */
        public Gadget process(GadgetContext context)
        {
            URI url = context.getUrl();

            if (url == null)
            {
                throw new ProcessingException("Missing or malformed url parameter");
            }

            if (!url.Scheme.ToLower().Equals("http") && !url.Scheme.ToLower().Equals("https"))
            {
                throw new ProcessingException("Unsupported scheme (must be http or https).");
            }

            if (blacklist.isBlacklisted(context.getUrl()))
            {
                throw new ProcessingException("The requested gadget is unavailable");
            }

            try
            {
                GadgetSpec spec = gadgetSpecFactory.getGadgetSpec(context);
                spec = substituter.substitute(context, spec);

                return new Gadget()
                        .setContext(context)
                        .setSpec(spec)
                        .setCurrentView(getView(context, spec));
            } 
            catch (GadgetException e) 
            {
                throw new ProcessingException(e.Message, e);
            }
        }

        /**
        * Attempts to extract the "current" view for the given gadget.
        */
        private View getView(GadgetContext context, GadgetSpec spec) 
        {
            String viewName = context.getView();
            View view = spec.getView(viewName);
            if (view == null)
            {
                JsonArray aliases = containerConfig.getJsonArray(context.getContainer(),
                        "gadgets.features/views/" + viewName + "/aliases");
                if (aliases != null)
                {
                    for (int i = 0, j = aliases.Length; i < j; ++i)
                    {
                        viewName = aliases.GetString(i);
                        if (!String.IsNullOrEmpty(viewName)) 
                        {
                            view = spec.getView(viewName);
                            if (view != null) 
                            {
                                break;
                            }
                        }
                    }
                }

                if (view == null) 
                {
                    view = spec.getView(GadgetSpec.DEFAULT_VIEW);
                }
            }
            return view;
        }
    }
}
