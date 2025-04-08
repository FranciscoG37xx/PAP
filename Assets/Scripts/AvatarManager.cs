 using System.IO; // Para manipular ficheiros 
using UnityEngine;
using UnityEngine.UI; // Para componentes de UI
using SFB; // Biblioteca Standalone File Browser para o explorador de ficheiros

public class AvatarManager : MonoBehaviour
{
    public Button avatarButton; // Botão para selecionar o avatar
    public Image avatarImage;   // Imagem onde o avatar será exibido
    private bool isFileBrowserOpen = false; // Controle para evitar múltiplas aberturas

    void Start()
    {
        // Verificar e configurar os campos necessários
        if (avatarButton == null)
        {
            Debug.LogError("avatarButton não foi atribuído no Inspector! Certifique-se de arrastar o botão para o campo no script.");
            return;
        }

        if (avatarImage == null)
        {
            Debug.LogError("avatarImage não foi atribuído no Inspector! Certifique-se de arrastar a imagem para o campo no script.");
            return;
        }

        // Remover eventos duplicados antes de adicionar o evento
        avatarButton.onClick.RemoveListener(OpenFileExplorer);
        avatarButton.onClick.AddListener(OpenFileExplorer);
    }

    public void OpenFileExplorer()
    {
        // Evitar múltiplas aberturas do explorador
        if (isFileBrowserOpen)
        {
            Debug.LogWarning("O explorador já está aberto. Ignorando nova solicitação.");
            return;
        }

        isFileBrowserOpen = true; // Marcar como aberto
        Debug.Log("Explorador de ficheiros aberto.");

        // Abrir o explorador de ficheiros para selecionar imagens
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Escolha uma imagem", "", "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg", false);

        if (paths == null || paths.Length == 0 || string.IsNullOrEmpty(paths[0]))
        {
            Debug.LogWarning("Nenhum ficheiro foi selecionado ou o explorador foi fechado.");
            isFileBrowserOpen = false; // Liberar a flag
            return;
        }

        string path = paths[0];
        Debug.Log($"Caminho selecionado: {path}");

        // Verificar se o ficheiro existe
        if (System.IO.File.Exists(path))
        {
            LoadImage(path);
        }
        else
        {
            Debug.LogError("O ficheiro selecionado não existe!");
        }

        // Garantir que o explorador foi fechado
        isFileBrowserOpen = false;
    }

    private void LoadImage(string path)
    {
        try
        {
            // Ler os bytes do ficheiro e criar uma textura
            byte[] imageBytes = System.IO.File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);

            if (texture.LoadImage(imageBytes))
            {
                Debug.Log("Imagem carregada com sucesso. Atualizando o avatar...");

                // Atualizar o sprite do avatar
                if (avatarImage != null)
                {
                    avatarImage.sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)
                    );
                    Debug.Log("Avatar atualizado com sucesso.");
                }
                else
                {
                    Debug.LogError("avatarImage está nulo. Verifique se está atribuído no Inspector.");
                }
            }
            else
            {
                Debug.LogWarning("Falha ao carregar a textura da imagem.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Erro ao carregar a imagem: {ex.Message}");
        }
    }
}



