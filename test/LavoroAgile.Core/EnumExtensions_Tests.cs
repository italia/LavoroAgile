using Domain.Model.Utilities;
using System;
using Xunit;

namespace LavoroAgile.Core
{
    /// <summary>
    /// Test unitari sui metodi di estensioni offerti da <see cref="EnumExtensions"/>.
    /// </summary>
    public class EnumExtensions_Tests
    {
        /// <summary>
        /// Tentativo di convertire un ruolo corretto.
        /// </summary>
        [Fact]
        public void Convert_Success()
        {
            var expected = RoleAndKeysClaimEnum.KEY_CLAIM_RESPONSABILE_ACCORDO;

            var input = expected.ToDescriptionString();

            var actual = input.ToEnum<RoleAndKeysClaimEnum>();

            Assert.Equal(expected, actual);

        }

        /// <summary>
        /// Tentativo di convertire un ruolo non valido.
        /// </summary>
        [Fact]
        public void Convert_NotValid()
        {
            var input = "Not valid description";

            Assert.Throws<InvalidCastException>(() => input.ToEnum<RoleAndKeysClaimEnum>());

        }

    }

}
