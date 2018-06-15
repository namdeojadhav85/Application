using Application.Common.Logging;
using Application.DTO;
using Application.Manager.Contract;
using Application.Manager.Resources;
using Application.Repository.ProfileModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Application.Manager.Conversion;
using Application.Core.ProfileModule.ProfileAggregate;
using Application.Common.Validator;
using Application.Common;
using Application.DTO.ProfileModule;

namespace Application.Manager.Implementation
{
    public class ContactManager : IContactManager
    {
        #region Global Declearation

        ProfileRepository _profileRepository;

        #endregion Global Declearation

        #region Constructor

        public ContactManager(ProfileRepository profileRepository)
        {
            if (profileRepository == null)
                throw new ArgumentNullException("profileRepository");
            _profileRepository = profileRepository;
        }

        #endregion Constructor

        #region Interface Implementation

        /// <summary>
        /// Get all Profiles
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public List<ProfileDTO> FindProfiles(int pageIndex, int pageCount)
        {
            if (pageIndex < 0 || pageCount <= 0)
                throw new ArgumentException(Messages.warning_InvalidArgumentForFindProfiles);

            //recover profiles in paged fashion
            var profiles = _profileRepository.GetPaged<DateTime>(pageIndex, pageCount, o => o.Created, false);
            if (profiles != null
                &&
                profiles.Any())
            {
                List<ProfileDTO> lstProfileDTO = new List<ProfileDTO>();
                foreach (var profile in profiles)
                {
                    lstProfileDTO.Add(Conversion.Mapping.ProfileToProfileDTO(profile));//, addressTypes, phoneTypes));
                }
                return lstProfileDTO;
            }
            else // no data
                return new List<ProfileDTO>();
        }

        /// <summary>
        /// Delete profile
        /// </summary>
        /// <param name="profileId"></param>
        public void DeleteProfile(int profileId)
        {
            var profile = _profileRepository.Get(profileId);

            if (profile != null) //if profile exist
            {
                _profileRepository.Remove(profile);

                //commit changes
                _profileRepository.UnitOfWork.Commit();
            }
            else //the customer not exist, cannot remove
                LoggerFactory.CreateLog().LogWarning(Messages.warning_CannotRemoveNonExistingProfile);
        }

        /// <summary>
        /// Find Profile by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProfileDTO FindProfileById(int id)
        {

            //recover orders
            var profile = _profileRepository.Get(id);
            
            if (profile != null)
            {
                return Conversion.Mapping.ProfileToProfileDTO(profile);
            }
            else //no data
                return new ProfileDTO();

        }

        /// <summary>
        /// Add new profile
        /// </summary>
        /// <param name="profileDTO"></param>
        /// <returns></returns>
        public void SaveProfileInformation(ProfileDTO profileDTO)
        {
            //if profileDTO data is not valid
            if (profileDTO == null)
                throw new ArgumentException(Messages.warning_CannotAddProfileWithNullInformation);

            //Create a new profile entity
            var newProfile = ProfileFactory.CreateProfile(profileDTO.FirstName, profileDTO.LastName, profileDTO.Email, profileDTO.Status, "Namdeo Jadhav", DateTime.Now, "Namdeo Jadhav", DateTime.Now);

            //Save Profile
            newProfile = SaveProfile(newProfile);

            
        }

        /// <summary>
        /// Update existing profile
        /// </summary>
        /// <param name="id"></param>
        /// <param name="profileDTO"></param>
        public void UpdateProfileInformation(int id, ProfileDTO profileDTO)
        {
            //if profileDTO data is not valid
            if (profileDTO == null)
                throw new ArgumentException(Messages.warning_CannotAddProfileWithNullInformation);

            //Create a new profile entity
            var currentProfile = _profileRepository.Get(id);

            //Assign updated value to existing profile
            var updatedProfile = new Profile();
            updatedProfile.ProfileId = id;
            updatedProfile.FirstName = profileDTO.FirstName;
            updatedProfile.LastName = profileDTO.LastName;
            updatedProfile.Email = profileDTO.Email;
            updatedProfile.Status = profileDTO.Status;

            //Update Profile
            updatedProfile = this.UpdateProfile(currentProfile, updatedProfile);

           
        }

        #endregion Interface Implementation

        #region Private Methods

        /// <summary>
        /// Save Profile
        /// </summary>
        /// <param name="profile"></param>
        Profile SaveProfile(Profile profile)
        {
            var entityValidator = EntityValidatorFactory.CreateValidator();

            if (entityValidator.IsValid(profile))//if entity is valid save. 
            {
                //add profile and commit changes
                _profileRepository.Add(profile);
                _profileRepository.UnitOfWork.Commit();
                return profile;
            }
            else // if not valid throw validation errors
                throw new ApplicationValidationErrorsException(entityValidator.GetInvalidMessages(profile));
        }

        /// <summary>
        /// Update existing Profile
        /// </summary>
        /// <param name="profile"></param>
        Profile UpdateProfile(Profile currentProfile, Profile updatedProfile)
        {
            updatedProfile.Created = currentProfile.Created;
            updatedProfile.CreatedBy = currentProfile.CreatedBy;
            updatedProfile.Updated = DateTime.Now;
            updatedProfile.UpdatedBy = "Namdeo Jadhav";

            var entityValidator = EntityValidatorFactory.CreateValidator();

            if (entityValidator.IsValid(updatedProfile))//if entity is valid save. 
            {
                //add profile and commit changes
                _profileRepository.Merge(currentProfile, updatedProfile);
                 _profileRepository.UnitOfWork.Commit();
                return updatedProfile;
            }
            else // if not valid throw validation errors
                throw new ApplicationValidationErrorsException(entityValidator.GetInvalidMessages(updatedProfile));
        }

        #endregion

    }
}
