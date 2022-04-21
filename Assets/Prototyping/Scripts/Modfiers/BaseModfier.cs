
namespace Prototyping.Scripts.Modfiers
{
    public class BaseModfier
    {
        public float value {get; set;}    
        public float lifeSpan {get; set;}
        
        public BaseModfier(float value, float lifeSpan)
        {
            this.value = value;
            this.lifeSpan = lifeSpan;
        }
    }
}

