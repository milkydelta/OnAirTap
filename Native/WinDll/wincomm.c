#include <stdio.h>
#include <tchar.h>

#include <windows.h>

typedef struct _dataBlock{
    char* name;
    int length;
    void* data;
    HANDLE fd;
} dataBlock;

int wopen(dataBlock* dst){

    if ((dst->fd = CreateFileMapping(INVALID_HANDLE_VALUE, 0, PAGE_READWRITE, 0, dst->length, dst->name)) == 0)
    {
        printf("Couldn't make filemapping\n");
        return -11;
    }

    if ((dst->data = MapViewOfFile(dst->fd, FILE_MAP_ALL_ACCESS, 0, 0, dst->length)) == 0)
    {
        printf("Couldn't make view\n");
        CloseHandle(dst->fd);
        return -12;
    }
    return 0;
}

int wclose(dataBlock* dst){
    UnmapViewOfFile(dst->data);
    CloseHandle(dst->fd);
    return 0;
}
