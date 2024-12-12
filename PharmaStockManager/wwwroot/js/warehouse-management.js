function openAddModal() {
    document.getElementById('modalTitle').textContent = 'Add New Warehouse';
    document.getElementById('warehouseForm').reset();
    document.getElementById('deleteButton').style.display = 'none';
    document.getElementById('warehouseId').value = '';
    document.getElementById('warehouseModal').classList.remove('hidden');
}

function openEditModal(id) {
    // This would typically be handled by an AJAX call to get warehouse details
    // For now, it's a placeholder
    fetch(`/SuperAdmin/GetWarehouse/${id}`)
        .then(response => response.json())
        .then(warehouse => {
            document.getElementById('modalTitle').textContent = 'Edit Warehouse';
            document.getElementById('warehouseId').value = warehouse.id;
            document.getElementById('Name').value = warehouse.name;
            document.getElementById('RefCode').value = warehouse.refCode;
            document.getElementById('Address').value = warehouse.address;
            document.getElementById('deleteButton').style.display = 'inline-block';
            document.getElementById('warehouseModal').classList.remove('hidden');
        });
}

function closeModal() {
    document.getElementById('warehouseModal').classList.add('hidden');
    document.getElementById('warehouseForm').reset();
}

function deleteWarehouse() {
    const id = document.getElementById('warehouseId').value;
    if (id) {
        fetch(`/SuperAdmin/DeleteWarehouse/${id}`, { method: 'POST' })
            .then(response => {
                if (response.ok) {
                    location.reload(); // Refresh the page
                }
            });
    }
}

// Add event listener for form submission
document.getElementById('warehouseForm').addEventListener('submit', function (e) {
    e.preventDefault();

    const formData = new FormData(this);

    fetch(this.action, {
        method: 'POST',
        body: formData
    }).then(response => {
        if (response.ok) {
            location.reload(); // Refresh the page
        }
    });
});