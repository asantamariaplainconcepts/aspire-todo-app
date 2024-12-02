<template>
  <form class="flex justify-center">
    <Fieldset legend="New todo">
      <InputText id="new-todo" v-model="newTodoTitle" aria-describedby="New Todo" />
      <Button @click="addTodo">Add</Button>
    </Fieldset>
  </form>

  <DataTable :value="todos" stripedRows tableStyle="min-width: 50rem">
    <Column field="title" header="Title"></Column>
    <Column field="isComplete" header="Is Complete">
      <template #body="slotProps">
        <ToggleSwitch v-model="slotProps.data.isCompleted" @click="updateTodo(slotProps.data)" />
      </template>
    </Column>
    <Column header="Actions">
      <template #body="slotProps">
        <Button @click="deleteTodo(slotProps.data.id)">Delete</Button>
      </template>
    </Column>
  </DataTable>
</template>

<script>
import axios from 'axios';


export default {
  data() {
    return {
      todos: [],
      newTodoTitle: ''
    };
  },
  methods: {
    async fetchTodos() {
      try {
        const response = await axios.get('api/todos');
        this.todos = response.data;
      } catch (error) {
        console.error('Error fetching todos:', error);
      }
    },
    async addTodo() {
      try {
        const newTodo = {title: this.newTodoTitle};
        await axios.post('api/todos', newTodo);
        this.newTodoTitle = '';
        await this.fetchTodos();
      } catch (error) {
        console.error('Error adding todo:', error);
      }
    },
    async updateTodo(todo) {
      try {
        await axios.put(`api/todos/${todo.id}/complete`);
        await this.fetchTodos();
      } catch (error) {
        console.error('Error updating todo:', error);
      }
    },
    async deleteTodo(id) {
      try {
        await axios.delete(`api/todos/${id}`);
        await this.fetchTodos();
      } catch (error) {
        console.error('Error deleting todo:', error);
      }
    }
  },
  mounted() {
    this.fetchTodos();
  }
};
</script>