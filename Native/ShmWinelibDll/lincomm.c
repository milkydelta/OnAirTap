#include "../header.h"

//shm_open and mmap
#include <sys/mman.h>
#include <sys/stat.h>
#include <fcntl.h>

//ftruncate
#include <unistd.h>
#include <sys/types.h>

#include <windef.h>

int WINAPI lopen(dataBlock* dst){

    dst->fd=shm_open(dst->name, O_CREAT|O_RDWR, S_IRUSR|S_IWUSR);
    if (dst->fd < 0)
    {
        return -1;
    }

    if (ftruncate(dst->fd, dst->length) == -1) {
        return -2;
    }

    dst->data = mmap(nullptr, dst->length, PROT_READ|PROT_WRITE, MAP_SHARED , dst->fd, 0);
    if (dst->data == MAP_FAILED)
    {
        return -3;
    }

    return 0;

}

int WINAPI lclose(dataBlock* dst){
    if (dst->data != nullptr){
        munmap(dst->data, dst->length);
        dst->data = nullptr;
    }
    if (dst->fd != 0){
        shm_unlink(dst->name);
        dst->fd = 0;
    }
    return 0;
}
