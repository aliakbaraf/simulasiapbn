/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */

using SimulasiAPBN.Common.Serializer;

namespace SimulasiAPBN.Web.Models.Engine
{
    public class EngineError
    {
        public EngineError() { }

        public EngineError(EngineErrorCode code, string reason)
        {
            Code = code;
            Reason = reason;
        }

        public EngineError(EngineErrorCode code, string reason, string supportId)
        {
            Code = code;
            Reason = reason;
            SupportId = supportId;
        }
        
        public EngineErrorCode Code { get; set; }

        public string Reason { get; set; }
        
        public string SupportId { get; set; }

        public EngineResponse FormResponse()
        {
            return new EngineResponse(this);
        }
        
        public override string ToString()
        {
            return Json.Serialize(this);
        }
    }
}
