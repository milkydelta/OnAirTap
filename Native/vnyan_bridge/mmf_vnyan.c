#include <stdio.h>
#include <tchar.h>

#include "../header.h"

#include <windows.h>


HANDLE LumMappingHandle;
LIVnyan_dat* LumMmf;
dataBlock shm_dat;
LIVnyan_dat* LumShm;


typedef int (__stdcall *shmFunc)(dataBlock*);

int ConnectMMF()
{
    if ((LumMappingHandle = CreateFileMapping(INVALID_HANDLE_VALUE, 0, PAGE_READWRITE, 0, sizeof(LIVnyan_dat), "Local\\uk.lum.livnyan.cameradata.v1.0")) == 0)
    {
        printf("made filemapping\n");
        return -11;
    }

    if ((LumMmf = (LIVnyan_dat*)MapViewOfFile(LumMappingHandle, FILE_MAP_ALL_ACCESS, 0, 0, sizeof(LIVnyan_dat))) == 0)
    {
        printf("made view\n");
        return -12;
    }

    return 0;

}

int ConnectSHM(){
    HINSTANCE dllHandle = LoadLibrary("lincomm.dll");

    if (!dllHandle) {
        printf("Couldn't load lincomm.dll'\n");
        return -21;
    }

    shmFunc lopen = (shmFunc)GetProcAddress(dllHandle, "open");
    if (!lopen) {
        printf("Couldn't find lincomm open()'\n");
        return -22;
    }

    shmFunc lclose = (shmFunc)GetProcAddress(dllHandle, "close");
    if (!lclose) {
        printf("Couldn't find lincomm close()\n");
        return -23;
    }

    char* strr = "/uk.lum.livnyan.cameradata.v1.0";

    shm_dat.name = strr;
    shm_dat.length = sizeof(LIVnyan_dat);

    int res = lopen(&shm_dat);
    if (res < 0){
        printf("lopen returned < 0\n");
        return res;
    }

    LumShm = (LIVnyan_dat*)shm_dat.data;
}


int main()
{
    int res;

    res = ConnectMMF();
    if (res < 0) {return res;}

    res = ConnectSHM();
    if (res < 0) {return res;}

    while (true) {
        *LumShm = *LumMmf;
        Sleep(1);
    };
    return 0;
}
