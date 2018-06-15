using Application.Core.ProfileModule.ProfileAggregate;
using Application.DTO;
using Application.DTO.ProfileModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Manager.Conversion
{
    public static class Mapping
    {
        public static ProfileDTO ProfileToProfileDTO(Profile profile)//, List<AddressType> addressTypes, List<PhoneType> phoneTypes)
        {
            ProfileDTO objProfileDTO = new ProfileDTO();
            objProfileDTO.ProfileId = profile.ProfileId;
            objProfileDTO.FirstName = profile.FirstName;
            objProfileDTO.LastName = profile.LastName;
            objProfileDTO.Email = profile.Email;
            objProfileDTO.Status = profile.Status;
            return objProfileDTO;
        }
       
    }
}

