using System.Collections.Generic;

namespace App1.Dao
{
    public interface CrudDao<Id, E>
    {
        E FindById(Id id);

        List<E> FindAll();

        void Save(E e);

        void Update(E e);

        void DeleteById(Id id);
    }

}
