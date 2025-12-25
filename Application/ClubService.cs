using System.Collections.Generic;
using StudentSystem.Core;
using StudentSystem.Infrastructure;

namespace StudentSystem.Application
{
    public class ClubService
    {
        private readonly ClubRepository _repository;

        public ClubService()
        {
            _repository = new ClubRepository();
        }

        public List<Club> GetAllClubs() => _repository.GetAll();

        public List<Student> GetClubMembers(int clubId) => _repository.GetMembers(clubId);

        public void CreateClub(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new System.Exception("Club name cannot be empty!");

            _repository.Add(name, description);
        }
    }
}