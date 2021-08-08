/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System.ComponentModel.DataAnnotations;
using SimulasiAPBN.Common.Serializer;

namespace SimulasiAPBN.Web.Models.Engine
{

    public class EngineResponse <T>
    {

        public EngineResponse()
        {
            Success = true;
        }

        public EngineResponse(T data)
        {
            Success = true;
            Data = data;
        }

        public EngineResponse(EngineError error)
        {
            Success = false;
            Error = error;
        }

        [Required]
        public bool Success { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }

        public EngineError Error { get; set; }

        public override string ToString()
        {
            return Json.Serialize(this);
        }
    }

    public class EngineResponse : EngineResponse<object> 
    {

        public EngineResponse() { }

        public EngineResponse(object data) : base(data) { }

        public EngineResponse(EngineError error) : base(error) { }

    }

}
