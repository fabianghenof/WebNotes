var saidHi = localStorage.getItem('saidHi');

if (saidHi !== 'true') {
    toastr.success('Hey, welcome! :)');
    localStorage.setItem('saidHi', 'true');
}