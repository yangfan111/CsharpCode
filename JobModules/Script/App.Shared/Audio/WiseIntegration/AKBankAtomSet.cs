using System.Collections.Generic;

namespace App.Shared.Audio
{
    public delegate void WiseReusltHandler(AKRESULT akresult);

    public class AKBankAtomSet
    {
        private readonly Dictionary<string, AKBankAtom> bankAtomContenter = new Dictionary<string, AKBankAtom>();

        
        public AKBankAtom Register(string bnkName, AudioBank_LoadMode modeType)
        {
            AKBankAtom atom;
            if (!bankAtomContenter.TryGetValue(bnkName, out atom))
            {
                atom = new AKBankAtom(bnkName, modeType);
                bankAtomContenter.Add(bnkName, atom);
            }

            return atom;
        }

        public void UnloadAll()
        {
            /*foreach (var bankAtom in bankAtomContenter)
            {
                bankAtom.
            }*/
            
        }
        public AKBankAtom Get(string bnk)
        {
            AKBankAtom atom;
            bankAtomContenter.TryGetValue(bnk, out atom);
            return atom;
        }

        public void DoLoadBank(AKBankAtom atom, WiseReusltHandler handler = null)
        {
            AKRESULT akresult;
            if (atom.Execute(handler, out akresult))
            {
                if (handler != null)
                    handler(akresult);
                else
                    AudioUtil.VerifyAKResult(akresult, "load atom:{0}", atom.BankName);
            }
        }

        private void DefaultLoadAyncHandler()
        {
        }
    }
}