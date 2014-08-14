#include "MUserList.h"

using namespace mega;
using namespace Platform;

MUserList::MUserList(UserList *userList, bool cMemoryOwn)
{
	this->userList = userList;
	this->cMemoryOwn = cMemoryOwn;
}

MUserList::~MUserList()
{
	if (cMemoryOwn)
		delete userList;
}

MUser^ MUserList::get(int i)
{
	return userList ? ref new MUser(userList->get(i), false) : nullptr;
}

int MUserList::size()
{
	return userList ? userList->size() : 0;
}

