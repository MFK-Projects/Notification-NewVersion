using System.Collections.Generic;

namespace MFKIanApi.Models
{
    internal class RootModel<TEntity> where TEntity:class
    {
        public List<TEntity> Values { get; set; }
    }
}
