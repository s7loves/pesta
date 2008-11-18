using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pesta
{
    public class VariableSubstituter
    {
        private readonly MessageBundleFactory messageBundleFactory;

        public VariableSubstituter()
        {
            this.messageBundleFactory = DefaultMessageBundleFactory.Instance;
        }

        /**
        * Substitutes all hangman variables into the gadget spec.
        *
        * @return A new GadgetSpec, with all fields substituted as needed.
        */
        public GadgetSpec substitute(GadgetContext context, GadgetSpec spec) 
        {
            MessageBundle bundle =
                    messageBundleFactory.getBundle(spec, context.getLocale(), context.getIgnoreCache());
            String dir = bundle.getLanguageDirection();

            Substitutions substituter = new Substitutions();
            substituter.addSubstitutions(Substitutions.Type.MESSAGE, bundle.getMessages());
            BidiSubstituter.addSubstitutions(substituter, dir);
            substituter.addSubstitution(Substitutions.Type.MODULE, "ID",
                        context.getModuleId().ToString());
            UserPrefSubstituter.addSubstitutions(substituter, spec, context.getUserPrefs());

            return spec.substitute(substituter);
        }
    }
}
