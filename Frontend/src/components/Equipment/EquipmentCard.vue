<template>
  <tr>
    <td>{{ code }}</td>
    <td>{{ name }}</td>
    <td>{{ type }}</td>
    <td>{{ status }}</td>
  </tr>
</template>

<script setup>
defineProps({ code: String, name: String, type: String, status: String })
</script>
