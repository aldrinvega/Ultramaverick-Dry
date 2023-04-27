using System;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.EXCEPTIONS
{
    public class NoResultFound : Exception
    {
        public NoResultFound() : base($"No Result found"){}
    }
}