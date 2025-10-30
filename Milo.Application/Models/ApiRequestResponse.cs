using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;

namespace Milo.Application.Models
{
    [DataContract]
    public class ApiRequestResponse : IEquatable<ApiRequestResponse>
    {
        [DataMember(Name = "code")]
        public long? Code { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "systemMessage")]
        public string SystemMessage { get; set; }

        [DataMember(Name = "stacTrace")]
        public string StackTrace { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ApiRequestResponse {\n");
            sb.Append(" Code: ").Append(Code).Append("\n");
            sb.Append(" Type: ").Append(Type).Append("\n");
            sb.Append(" Message: ").Append(Message).Append("\n");
            sb.Append(" SystemMessage: ").Append(SystemMessage).Append("\n");
            sb.Append(" StackTrace: ").Append(StackTrace).Append("\n");
            return sb.ToString();
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ApiRequestResponse)obj);
        }

        public bool Equals(ApiRequestResponse other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                Code == other.Code ||
                Code != null &&
                Code.Equals(other.Code)
                ) &&
                (Type == other.Type ||
                Type != null &&
                Type.Equals(other.Type)
                ) &&
                (Message == other.Message ||
                Message != null &&
                Message.Equals(other.Message)
                ) &&
                (SystemMessage == other.SystemMessage ||
                SystemMessage != null &&
                SystemMessage.Equals(other.SystemMessage)
                ) &&
                (StackTrace == other.StackTrace ||
                StackTrace != null &&
                StackTrace.Equals(other.StackTrace)
                );
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hasCode = 41;
                if (Code != null)
                    hasCode = hasCode * 59 + Code.GetHashCode();
                if (Type != null)
                    hasCode = hasCode * 59 + Type.GetHashCode();
                if (Message != null)
                    hasCode = hasCode * 59 + Message.GetHashCode();
                if (SystemMessage != null)
                    hasCode = hasCode * 59 + SystemMessage.GetHashCode();
                if (StackTrace != null)
                    hasCode = hasCode * 59 + StackTrace.GetHashCode();
                return hasCode;
            }
        }

        public static bool operator ==(ApiRequestResponse left, ApiRequestResponse right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ApiRequestResponse left, ApiRequestResponse right)
        {
            return !Equals(left, right);
        }
    }
}
